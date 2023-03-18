using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.SeverClass;
using KEDI.Core.Premise.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CKBS.Controllers
{
    [Privilege]
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRate _IExchang;
        private readonly DataContext _context;

        public ExchangeRateController(IExchangeRate exchangeRate,DataContext dataContext)
        {
            _IExchang = exchangeRate;
            _context = dataContext;
        }

        [HttpGet]
        // GET: ExchangeRate
        [Privilege("A014")]
        public IActionResult Index()
        {
            string name="";
            ViewBag.style = "fa fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Exchange Rate";
            ViewBag.Subpage = "List";
            ViewBag.Banking = "show";
            ViewBag.ExchangeRate = "highlight";
            IEnumerable<Currency> currencies = _IExchang.GetBaseCurrencyName();
            foreach (var item in currencies as IEnumerable<Currency>)
            {
                name = item.Description;

            }
            ViewData["BaseCurrencyName"] = name;
            ViewData["Currency"] = _IExchang.GetListCurrency();
            return View();
        }

        [HttpPost]
        [Privilege("A014")]
        public IActionResult Edit(ExchangeRateService data)
        {
            if (ModelState.IsValid)
            {
                _IExchang.UpdateExchangeRate(data);              
            }
            return Ok(data);
        }
    }
}
