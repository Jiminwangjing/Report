using System;
using System.Collections.Generic;
using System.IO;
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

namespace CKBS.Controllers
{
    [Privilege]
    public class ReceiptInformationController : Controller
    {
        private readonly DataContext _context;
        private readonly IReceiptInformation _receiptInformation;
        private readonly IWebHostEnvironment _appEnvironment;

        public ReceiptInformationController(DataContext context, IReceiptInformation receiptInformation, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _receiptInformation = receiptInformation;
            _appEnvironment = hostingEnvironment;
        }

        [Privilege("A003")]
        public IActionResult Index()
        {
            ViewBag.Receipt = "highlight";
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

        public IActionResult GetReceiptInformation(string keyword = "")
        {
            var getReceiptInformation = from re in _context.ReceiptInformation
                                        join b in _context.Branches.Where(x => x.Delete == false) on re.BranchID equals b.ID
                                        select new
                                        {
                                            ID = re.ID,
                                            BranchID = re.BranchID,
                                            Title = re.Title,
                                            Branch = b.Name,
                                            Tel1 = re.Tel1,
                                            KhmerDescription = re.KhmerDescription,
                                            EnglishDescription = re.EnglishDescription,
                                        };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                getReceiptInformation = getReceiptInformation.Where(c => RawWord(c.Title).Contains(keyword, ignoreCase));
            }
            return Ok(getReceiptInformation.ToList());
        } 

        // GET: ReceiptInformation/Create
        [Privilege("A003")]
        public IActionResult Create()
        {           
            ViewBag.Receipt = "highlight";

            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Title2,Address,Address2,Tel1,Tel2,Email,VatTin,KhmerDescription,EnglishDescription,PowerBy,Logo,BranchID,TeamCondition,TeamCondition2,Website")] ReceiptInformation receiptInformation)
        {            
            ViewBag.Receipt = "highlight";
            if (receiptInformation.BranchID == 0 || receiptInformation.BranchID==null)
            {
               ViewBag.required_branch = "Please select branch !";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var file = Image;
                            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Logo");
                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    receiptInformation.Logo = fileName;
                                }

                            }
                        }
                    }
                    await _receiptInformation.AddOrEdit(receiptInformation);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View(receiptInformation);
        }

        // GET: ReceiptInformation/Edit/5
        [Privilege("A003")]
        public async Task<IActionResult> Edit(int? id)
        {            
            ViewBag.Receipt = "highlight";
            if (id == null)
            {
                return NotFound();
            }

            var receiptInformation = await _context.ReceiptInformation.FindAsync(id);
            if (receiptInformation == null)
            {
                return NotFound();
            }
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View(receiptInformation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Title2,Address,Address2,Tel1,Tel2,Email,VatTin,KhmerDescription,EnglishDescription,PowerBy,Logo,BranchID,TeamCondition,TeamCondition2,Website")] ReceiptInformation receiptInformation)
        {            
            ViewBag.Receipt = "highlight";

            if (id != receiptInformation.ID)
            {
                return NotFound();
            }
            if (receiptInformation.BranchID == 0 || receiptInformation.BranchID == null)
            {
                ViewBag.required_branch = "Please select branch !";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var files = HttpContext.Request.Form.Files;
                        foreach (var Image in files)
                        {
                            if (Image != null && Image.Length > 0)
                            {
                                var file = Image;
                                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Logo");
                                if (file.Length > 0)
                                {
                                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                        receiptInformation.Logo = fileName;
                                    }

                                }
                            }
                        }
                        await _receiptInformation.AddOrEdit(receiptInformation);
                    }
                    catch (Exception)
                    {
                       
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View(receiptInformation);
        }

        // POST: ReceiptInformation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receiptInformation = await _context.ReceiptInformation.FindAsync(id);
            _context.ReceiptInformation.Remove(receiptInformation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptInformationExists(int id)
        {
            return _context.ReceiptInformation.Any(e => e.ID == id);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddRecirptInfoma(ReceiptInformation receipt)
        {
           await _receiptInformation.AddOrEdit(receipt);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRescirtp()
        {
            var list = _receiptInformation.GetReceiptInformation().OrderByDescending(x=>x.ID).ToList();
            return Ok(list);
        }
    }
}
