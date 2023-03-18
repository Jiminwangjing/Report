using CKBS.AppContext;
using CKBS.Models.Services.Administrator.SystemInitialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class PeriodIndicatorController : Controller
    {
        private readonly DataContext _context;
        public PeriodIndicatorController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.PeriodIndicator = "highlight";
            var indecator = _context.PeriodIndicators.Where(i => !i.Delete && i.CompanyID == GetCompanyID()).ToList();
            return View(indecator);
            
        }

        int GetCompanyID()
        {
            int.TryParse(User.FindFirstValue("CompanyID"), out int _companyId);
            return _companyId;
        }

        public IActionResult Create()
        {        
            ViewBag.PeriodIndicator = "highlight";
            return View();
        }

        public async Task<IActionResult> Add(PeriodIndicator periodIndicator)
        {
            periodIndicator.CompanyID = GetCompanyID();
            if (ModelState.IsValid)
            {
                _context.Add(periodIndicator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(periodIndicator);
        }
      

        public IActionResult Edit(int ID)
        {
            var periodIndicator = _context.PeriodIndicators.FirstOrDefault(x => x.ID == ID);
            ViewBag.PeriodIndicator = "highlight";
            ViewData["PeriodIndicator"] = _context.PeriodIndicators.FirstOrDefault(i => i.ID == periodIndicator.ID);
           
            if (ID == 0)
            {
                return NotFound();
            }
            if (periodIndicator == null)
            {
                return NotFound();
            }
            return View(periodIndicator);
        }

        [HttpPost]
        public IActionResult Edit(PeriodIndicator periodIndicator)
        {
            if (ModelState.IsValid)
            {
                //periodIndicator.Delete = true;
                _context.Update(periodIndicator);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(periodIndicator);
        }

        public IActionResult Delete(int id)
        {
            var delete = _context.PeriodIndicators.Find(id);
            delete.Delete = true;
            _context.Update(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
