using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CKBS.AppContext;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class SystemTypesController : Controller
    {
        private readonly DataContext _context;

        public SystemTypesController(DataContext context)
        {
            _context = context;
        }

        // GET: SystemTypes
        public async Task<IActionResult> Index()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "List";
            ViewBag.Subpage = "";
            return View(await _context.SystemType.ToListAsync());
        }

        [HttpGet]   
        public IActionResult GetSystemType()
        {
            var system = _context.SystemType.Where(w => w.Status == true);
            return Ok(system);
        }

        // GET: SystemTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Detail";
            ViewBag.Subpage = "";
            if (id == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemType
                .FirstOrDefaultAsync(m => m.ID == id);
            if (systemType == null)
            {
                return NotFound();
            }

            return View(systemType);
        }

        // GET: SystemTypes/Create
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Create";
            ViewBag.Subpage = "";
            return View();
        }

        // POST: SystemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Type,Status")] SystemType systemType)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Create";
            if (ModelState.IsValid)
            {
                _context.Add(systemType);
                await _context.SaveChangesAsync();
                //await UpdateClaimsAsync(User, systemType);
                return RedirectToAction(nameof(Index));
            }
            return View(systemType);
        }

        private async Task<IActionResult> UpdateClaimsAsync(ClaimsPrincipal user, SystemType system) {
            ClaimsIdentity identity = user.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Claim claim = user.Claims.FirstOrDefault(c => c.Type == system.Type);
                //identity.RemoveClaim(claim);
                var existed = user.FindFirst("KernelSystem");
                JToken token = JToken.Parse(existed.Value);
                token[system.Type] = system.Status;
              
                identity.AddClaim(new Claim("KernelSystem", token.ToString()));
                var props = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddHours(24)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: SystemTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Edit";
            if (id == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemType.FindAsync(id);
            if (systemType == null)
            {
                return NotFound();
            }

            return View(systemType);
        }

        // POST: SystemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Type,Status")] SystemType systemType)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Edit";
            if (id != systemType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Logout", "Account");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemTypeExists(systemType.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(systemType);
        }

        // GET: SystemTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Delete";
            if (id == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemType
                .FirstOrDefaultAsync(m => m.ID == id);
            if (systemType == null)
            {
                return NotFound();
            }

            return View(systemType);
        }

        // POST: SystemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "System Type";
            ViewBag.Page = "Delete";
            var systemType = await _context.SystemType.FindAsync(id);
            _context.SystemType.Remove(systemType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemTypeExists(int id)
        {
            return _context.SystemType.Any(e => e.ID == id);
        }
    }
}
