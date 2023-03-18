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
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class CurrencyController : Controller
    {
        private readonly DataContext _context;
        private readonly ICurrency _currency;

        public CurrencyController(DataContext context,ICurrency currency)
        {
            _context = context;
            _currency = currency;
        }

        public int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
            return _userId;
        }

        [Privilege("A013")]
        public IActionResult Index()
        {
            ViewBag.Currency = "highlight";
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

        public IActionResult GetCurrencies(string keyword = "")
        {
            int userid = GetUserID();
            var currencies = _context.Currency.Where(g => g.Delete == false);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                currencies = currencies.Where(c => RawWord(c.Symbol).Contains(keyword, ignoreCase));
            }
            return Ok(currencies.ToList());
        }

        [Privilege("A013")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Currency";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.Currency = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Symbol,Description,Delete")] Currency currency)
        {
            ViewBag.style = "fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Currency";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.Currency = "highlight";

            if (ModelState.IsValid)
            {
                await _currency.AddorEdit(currency);
                return RedirectToAction(nameof(Index));
            }
            return View(currency);
        }

        [Privilege("A013")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Currency";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Banking = "show";
            ViewBag.Currency = "highlight";
            var currency = _currency.GetId(id);
            if (currency.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (currency == null)
            {
                return NotFound();
            }
            return View(currency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Symbol,Description,Delete")] Currency currency)
        {
            ViewBag.style = "fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Currency";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Banking = "show";
            ViewBag.Currency = "highlight";

            if (ModelState.IsValid)
            {
                try
                {
                    await _currency.AddorEdit(currency);
                }
                catch (Exception)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(currency);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _currency.Delete(id);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCurrency()
        {
            var list = _currency.Cuurencies().OrderByDescending(x=>x.ID).ToList();
            return Ok(list);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddCurrency(Currency currency)
        {
           await _currency.AddorEdit(currency);
           return Ok();
        }

    }
}
