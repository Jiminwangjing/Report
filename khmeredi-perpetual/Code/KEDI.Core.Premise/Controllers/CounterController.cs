using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class CounterController : Controller
    {
        private readonly DataContext _context;

        public CounterController(DataContext context)
        {
            _context = context;
        }

        // GET: Counter
        public async Task<IActionResult> Index()
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Counter";
            ViewBag.Administrator = "show";
            ViewBag.General = "show";
            ViewBag.Counter = "highlight";
            var dataContext = _context.Counters.Include(c => c.PrinterName);
            return View(await dataContext.ToListAsync());
        }


        // GET: Counter/Create
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Counter";
            ViewBag.Administrator = "show";
            ViewBag.General = "show";
            ViewBag.Counter = "highlight";
            ViewData["PrinterID"] = new SelectList(_context.PrinterNames, "ID", "Name");
            return View();
        }

        // POST: Counter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Delete,PrinterID")] Counter counter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(counter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrinterID"] = new SelectList(_context.PrinterNames, "ID", "Name", counter.PrinterID);
            return View(counter);
        }

        // GET: Counter/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var counter = await _context.Counters.FindAsync(id);
            if (counter == null)
            {
                return NotFound();
            }
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Counter";
            ViewBag.Administrator = "show";
            ViewBag.General = "show";
            ViewBag.Counter = "highlight";
            ViewData["PrinterID"] = new SelectList(_context.PrinterNames, "ID", "Name", counter.PrinterID);
            return View(counter);
        }

        // POST: Counter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Delete,PrinterID")] Counter counter)
        {
            if (id != counter.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(counter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CounterExists(counter.ID))
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
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Counter";
            ViewBag.Administrator = "show";
            ViewBag.General = "show";
            ViewBag.Counter = "highlight";
            ViewData["PrinterID"] = new SelectList(_context.PrinterNames, "ID", "Name", counter.PrinterID);
            return View(counter);
        }

        // GET: Counter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var counter = await _context.Counters
                .Include(c => c.PrinterName)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (counter == null)
            {
                return NotFound();
            }

            return View(counter);
        }
        
        private bool CounterExists(int id)
        {
            return _context.Counters.Any(e => e.ID == id);
        }
    }
}
