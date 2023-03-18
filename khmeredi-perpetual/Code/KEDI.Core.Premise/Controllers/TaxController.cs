using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class TaxController : Controller
    {
        private readonly DataContext _context;
        private readonly ITax _tax;
        private readonly IWebHostEnvironment _appEnvironment;

        public TaxController(DataContext context, ITax tax, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _tax = tax;
            _appEnvironment = hostingEnvironment;
        }

        [Privilege("A021")]
        public IActionResult Index()
        {
            ViewBag.CreateTax = "highlight";
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

        public IActionResult GetTax(string keyword = "")
        {
            var taxes = _context.Tax.Where(t=> t.Delete == false);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                taxes = taxes.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(taxes.ToList());
        }

        [Privilege("A021")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "VAT";
            ViewBag.Subpage = "";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.CreateTax = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Rate,Type,Effective,Delete")] Tax tax)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "VAT";
            ViewBag.Subpage = "";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.CreateTax = "highlight";
            if (ModelState.IsValid)
            {
                _context.Add(tax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tax);
        }

        // GET: Tax/Edit/5
        [Privilege("A021")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "VAT";
            ViewBag.Subpage = "";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.CreateTax = "highlight";
            if (id == null)
            {
                return NotFound();
            }

            var tax = await _context.Tax.FindAsync(id);
            if (tax == null)
            {
                return NotFound();
            }
            return View(tax);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Rate,Type,Effective,Delete")] Tax tax)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "VAT";
            ViewBag.Subpage = "";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Banking = "show";
            ViewBag.CreateTax = "highlight";
            if (id != tax.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tax);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxExists(tax.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tax);
        }
      
        private bool TaxExists(int id)
        {
            return _context.Tax.Any(e => e.ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            await _tax.DeleteTax(id);
            return Ok();
        }
    }
}
