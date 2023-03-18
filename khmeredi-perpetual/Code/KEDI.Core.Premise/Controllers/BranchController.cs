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
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.Account;
using KEDI.Core.Premise.Repository;

namespace CKBS.Controllers
{
    [Privilege]
    public class BranchController : Controller
    {
        private readonly DataContext _context;
        private readonly IBranch _branch;
         private readonly UserManager _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public BranchController(DataContext context, IBranch branch, IWebHostEnvironment env,UserManager userManager)
        {
            _context = context;
            _branch = branch;
            _appEnvironment = env;
             _userManager=userManager;
        }

        [Privilege("A002")]
        public IActionResult Index()
        {
            ViewBag.Branch = "highlight";
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

        public IActionResult GetBranch(string keyword = "")
        {
            var branches = from br in _context.Branches
                           join com in _context.Company on br.CompanyID equals com.ID
                           where !br.Delete
                           //join com in _context.Company.Where(br=>!br.Delete) on br.CompanyID equals com.ID
                           select new
                           {
                               ID = br.ID,
                               Name = br.Name,
                               Company = com.Name,
                               Location = br.Location,
                           };


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                branches = branches.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(branches.ToList());
        }
          public async Task<IActionResult> GetMultiBranch()
        {
            var list = (from mbr in _context.MultiBrands.Where(s => s.UserID==_userManager.CurrentUser.ID&& s.Active==true)
                        join br in _context.Branches on mbr.BranchID equals br.ID
                        select new MultiBrand
                        {
                            ID=br.ID,
                            Name= br.Name,
                            UserName =_userManager.CurrentUser.Username,
                            Location = br.Location,
                            Address = br.Address,
                            Active  = _userManager.CurrentUser.BranchID==mbr.BranchID?true:false,
                        });
            return Ok(await Task.FromResult(list));
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Branch";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.General = "show";
            ViewBag.Branch = "highlight";
            var userid = 0;
            int.TryParse(User.FindFirst("UserID").Value, out userid);
            // if (User.FindFirst("Password").Value == "YdQusX4G7SJ+txRJ2IZYDmx/L+s6SnnI4hQ+PqwCoDl09gtTubaDQiCfqhfDNYVn" && User.FindFirst("Username").Value == "Kernel")
            // {
            //     ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
            //     return View();
            // }
            var permision = _context.UserPrivilleges.FirstOrDefault(x => x.UserID == userid && x.Code == "A002");
            if (permision != null)
            {
                if (permision.Used == true)
                {
                    ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,CompanyID,Location,Address,Delete,Name2")] Branch branch)
        {
            ViewBag.General = "show";
            ViewBag.Branch = "highlight";
            if (branch.CompanyID == 0)
            {
                ViewData["error.company"] = "Please select company!";
            }
            if (branch.Name == null)
            {
                ViewData["error.Nane"] = "Please input name";
            }
            if (branch.Location == null)
            {
                ViewData["error.Location"] = "Please input location";
            }
            else
            {
                await _branch.AddOrEdit(branch);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
            return View(branch);
        }
        // GET: Branch/Edit/5
        [Privilege("A002")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Branch = "highlight";

            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
            return View(branch);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,CompanyID,Location,Address,Delete,Name2")] Branch branch)
        {


            if (id != branch.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var br = _context.Branches.FirstOrDefault(s => s.ID == id);

                    br.Name = branch.Name;
                    br.Name2 = branch.Name2;
                    br.CompanyID = branch.CompanyID;
                    br.Address = branch.Address;
                    br.Location = branch.Location;

                    await _branch.AddOrEdit(br);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.ID))
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
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
            return View(branch);
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            await _branch.Delete(id);
            return Ok();
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.FindAsync(id);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.ID == id);
        }
    }
}
