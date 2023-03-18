using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class FunctionsController : Controller
    {
        private readonly DataContext _context;

        public FunctionsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.style = "fas fa-cogs";
            ViewBag.Main = "Setting";
            ViewBag.Page = "Functions";
            ViewBag.Menu = "show";
            return View(await _context.Functions.OrderBy(o => o.Type).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.style = "fas fa-cogs";
            ViewBag.Main = "Functions";
            ViewBag.Page = "Create";
            ViewBag.Menu = "show";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Function function)
        {
            var functions = _context.Functions.Where(f => f.Code == function.Code).ToList();
            if (functions.Count > 0)
            {
                ModelState.AddModelError("Code", "Code is already exists");
            }
            if (ModelState.IsValid)
            {
                var t = _context.Database.BeginTransaction();
                if (!_context.Functions.Any(f => string.Compare(f.Code, function.Code, true) == 0))
                {
                    _context.Functions.Add(function);
                    await _context.SaveChangesAsync();

                    foreach (var user in _context.UserAccounts.Where(c => !c.Delete))
                    {
                        var _userP = new UserPrivillege
                        {
                            UserID = user.ID,
                            FunctionID = function.ID,
                            Code = function.Code,
                            Used = false,
                            Delete = false
                        };
                        _context.UserPrivilleges.Add(_userP);
                    }
                    _context.SaveChanges();
                }
                t.Commit();

                return RedirectToAction(nameof(Index));
            }
            return View(function);
        }

        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.style = "fas fa-cogs";
            ViewBag.Main = "Functions";
            ViewBag.Page = "Update";
            ViewBag.Menu = "show";
            if (id == null)
            {
                return NotFound();
            }

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                return NotFound();
            }
            return View(function);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Function function)
        {
            var functions = _context.Functions.Where(f => f.Code == function.Code && f.ID != function.ID).ToList();
            if (functions.Count > 0)
            {
                ModelState.AddModelError("Code", "Code is already exists");
            }
            if (id != function.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(function);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunctionExists(function.ID))
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
            return View(function);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var fun = await _context.Functions.FindAsync(id);
            _context.Functions.Remove(fun);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool FunctionExists(int id)
        {
            return _context.Functions.Any(e => e.ID == id);
        }
    }
}
