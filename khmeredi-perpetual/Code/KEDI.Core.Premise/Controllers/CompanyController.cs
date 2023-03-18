using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Responsitory;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using Microsoft.AspNetCore.Http;

namespace CKBS.Controllers
{
    [Privilege]
    public class CompanyController : Controller
    {
        private readonly DataContext _context;
        private readonly ICompany _company;
        private readonly IWebHostEnvironment _appEnvironment;
        public CompanyController(DataContext context, ICompany company, IWebHostEnvironment env)
        {
            _context = context;
            _company = company;
            _appEnvironment = env;
        }

        [Privilege("A001")]
        public IActionResult Index()
        {
            ViewBag.Company = "highlight";
            return View();
        }

        static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetCompanies(string keyword = "")
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
            var company = from us in _context.UserAccounts.Where(w => w.ID == _userId)
                          join br in _context.Branches on us.BranchID equals br.ID
                          join com in _context.Company on br.CompanyID equals com.ID
                          join ssc in _context.Currency on com.SystemCurrencyID equals ssc.ID
                          join lcc in _context.Currency on com.LocalCurrencyID equals lcc.ID
                          select new
                          {
                              com.ID,
                              com.Name,
                              com.Location,
                              com.Address,
                              SSC = ssc.Description,
                              LCC = lcc.Description,
                          };

            var companies = _context.Company.Where(c => !c.Delete);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                company = company.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(company.ToList());
        }
        // GET: Company        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,ReceiptID,Location,Address,Process,Delete,PriceListID,Logo,Logo2")] Company company)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Company";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.General = "show";
            ViewBag.Company = "highlight";

            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                UploadImg(company);
                return RedirectToAction(nameof(Index));

            }
            else
            {
                if (company.PriceListID == 0)
                    ViewData["error.receiptid"] = "Please select priceList !";

                if (company.Process == "0")
                    ViewData["error.Process"] = "Please select process !";

            }

            ViewData["PriceListID"] = new SelectList(_context.PriceLists.Where(c => c.Delete == false), "ID", "Name", company.PriceList);
            return View(company);
        }
        public void UploadImg(Company company)
        {
            try
            {

                var images = HttpContext.Request.Form.Files;
                if (images.Count > 0)
                {
                    if (images[0] != null)
                    {
                        CreateImage(images[0], company.Logo);
                    }

                    if (images.Count > 1 && images[1] != null)
                    {
                        CreateImage(images[1], company.Logo2);
                    }
                }


            }
            catch (Exception) { }
        }

        private void CreateImage(IFormFile image, string imgName)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Images/company");
            using var fs = new FileStream(Path.Combine(uploads, imgName), FileMode.Create);
            image.CopyTo(fs);
        }

        // GET: Company/Edit/5
        [Privilege("A001")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Company = "highlight";

            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            ViewData["PriceListID"] = new SelectList(_context.PriceLists.Where(c => c.Delete == false && c.ID == company.PriceListID), "ID", "Name", company.PriceList);
            ViewData["SystemCurrency"] = new SelectList(_context.Currency.Where(c => c.Delete == false && c.ID == company.SystemCurrencyID), "ID", "Description", company.SystemCurrencyID);
            ViewData["LocalCurrency"] = new SelectList(_context.Currency.Where(c => c.Delete == false && c.ID == company.LocalCurrencyID), "ID", "Description", company.LocalCurrencyID);
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,ReceiptID,Location,Address,Process,Delete,PriceListID,Logo,Logo2,SystemCurrencyID,LocalCurrencyID")] Company company)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Company";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.General = "show";
            ViewBag.Company = "highlight";

            if (id != company.ID)
            {
                return NotFound();
            }

            if (company.Process == "0")
            {
                ViewData["error.Process"] = "Please select process !";
                return View(company);
            }

            if (ModelState.IsValid)
            {
                 var cp = _context.Company.FirstOrDefault(s => s.ID == id);

                cp.Name = company.Name;
                cp.Location = company.Location;
                cp.Address = company.Address;
                cp.LocalCurrencyID = company.LocalCurrencyID;
                cp.SystemCurrencyID = company.SystemCurrencyID;
                cp.Logo = company.Logo;
                cp.Logo2 = company.Logo2;

                _company.UpdateCurrencyWarehouseDetail(company.PriceListID);
                _context.Update(cp);
                await _context.SaveChangesAsync();
                UploadImg(company);
                return RedirectToAction(nameof(Index));

            }
            else
            {
                if (company.PriceListID == 0)
                    ViewData["error.receiptid"] = "Please select priceList !";

                if (company.Process == "0")
                    ViewData["error.Process"] = "Please select process !";

            }
            ViewData["PriceListID"] = new SelectList(_context.PriceLists.Where(c => c.Delete == false), "ID", "Name", company.PriceList);

            return View(company);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            await _company.DeleteCompany(id);
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

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.ID == id);
        }
    }
}