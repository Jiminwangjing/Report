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
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class GroupTableController : Controller
    {
        private readonly DataContext _context;
        private readonly IGroupTable _groupTable;
        private readonly IWebHostEnvironment _appEnvironment;
        public GroupTableController(DataContext context, IGroupTable groupTable, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _groupTable = groupTable;
            _appEnvironment = hostingEnvironment;
        }

        [Privilege("A010")]
        public IActionResult Index()
        {
            ViewBag.TableGroup = "highlight";
            ViewData["GrouptableID"] = new SelectList(_context.GroupTables, "ID", "Name");
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

        public IActionResult GetGroupTable(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var groupTable = from grou in _context.GroupTables.Where(g => g.Delete == false)

                             select new
                             {
                                 ID = grou.ID,
                                 Name = grou.Name,
                                 Type = grou.Types,
                                 Image = grou.Image
                             };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                groupTable = groupTable.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(groupTable.ToList());
        }

        [Privilege("A010")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Group Table";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.TableMenu = "show";
            ViewBag.TableGroup = "highlight";
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,BranchID,Types,Image,Delete")] GroupTable groupTable)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Group Table";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.TableMenu = "show";
            ViewBag.TableGroup = "highlight";
            if (groupTable.Types == "0" || groupTable.Types == null)
            {
                ViewBag.required = "Please select type !";
                return View(groupTable);
            }
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        var uploads = Path.Combine(_appEnvironment.WebRootPath, "Images");
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                groupTable.Image = fileName;
                            }

                        }
                    }
                }
                await _groupTable.AddOrEdit(groupTable);
                return RedirectToAction(nameof(Index));
            }
            return View(groupTable);
        }

        [Privilege("A010")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Group Table";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.TableMenu = "show";
            ViewBag.TableGroup = "highlight";
            var groupTable = _groupTable.GetbyId(id);
            if (groupTable.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (groupTable == null)
            {
                return NotFound();
            }
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View(groupTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,BranchID,Types,Image,Delete")] GroupTable groupTable)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Group Table";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.TableMenu = "show";
            ViewBag.TableGroup = "highlight";
            if (groupTable.Types == "0" || groupTable.Types == null)
            {
                ViewBag.required = "Please select type !";
                return View(groupTable);
            }
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
                            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Images");
                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    groupTable.Image = fileName;
                                }

                            }
                        }
                    }
                    await _groupTable.AddOrEdit(groupTable);
                }
                catch (Exception)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchID"] = new SelectList(_context.Branches.Where(c => c.Delete == false), "ID", "Name");
            return View(groupTable);
        }

       [HttpPost]
       public async Task<IActionResult> DeleteGroupTable(int id)
        {
            await  _groupTable.Delete(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult QuickAdd(GroupTable groupTable)
        {
           _groupTable.InsertGroupTable(groupTable);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetDataGroupTable()
        {
            var list = _groupTable.GroupTables().OrderByDescending(s => s.ID);
            return Ok(list);
        }
    }
}
