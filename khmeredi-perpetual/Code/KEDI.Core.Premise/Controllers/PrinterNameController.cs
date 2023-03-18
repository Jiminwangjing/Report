using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class PrinterNameController : Controller
    {
        private readonly DataContext _context;
        private readonly IPrinterName _printer;

        public PrinterNameController(DataContext context,IPrinterName printerName)
        {
            _context = context;
            _printer = printerName;
        }

        [Privilege("A005")]
        public IActionResult Index()
        {
            ViewBag.PrinterName = "highlight";
            return View();
        }

        private static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetPrinterName(string keyword = "")
        {
            var printerName = _context.PrinterNames.Where(x => x.Delete == false);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                printerName = printerName.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(printerName.ToList());
        }

        [Privilege("A005")]
        public IActionResult Create()
        {
            ViewBag.PrinterName = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( PrinterName printerName)
        {
            ViewBag.PrinterName = "highlight";
            var count = 0;
            var printer = _context.PrinterNames.FirstOrDefault(s => s.Name == printerName.Name);
            if (printer != null)
            {
                ModelState.AddModelError("Name", "This Name already created !");
                count++;
            }
            if (count > 0)
            {
                return View(printerName);
            }
            if (ModelState.IsValid)
            {
                await _printer.AddorEdit(printerName);
                return RedirectToAction(nameof(Index));
            }
            return View(printerName);
        }

        [Privilege("A005")]
        public IActionResult Edit(int id)
        {
            ViewBag.PrinterName = "highlight";
            var printerName = _printer.GetbyId(id);
            if (printerName.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (printerName == null)
            {
                return NotFound();
            }
            return View(printerName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrinterName printerName)
        {
            var count = 0;
            ViewBag.PrinterName = "highlight";
            if (id != printerName.ID)
            {
                return NotFound();
            }
            var print = _context.PrinterNames.Where(s => s.ID != printerName.ID && s.Name == printerName.Name);
            if (print.Any())
            {
                ModelState.AddModelError("Name", "This Name already created !");
                count++;
            }
            if (count > 0)
            {
                return View(printerName);
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _printer.AddorEdit(printerName);
                }
                catch (Exception)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(printerName);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePrinter(int id)
        {
            await _printer.Delete(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult InsertPrinter(PrinterName printer)
        {
            var list = _printer.AddorEdit(printer);
            return Ok(list);
        }

    }
}

