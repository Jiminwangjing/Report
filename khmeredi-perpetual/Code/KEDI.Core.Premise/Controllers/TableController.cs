using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Responsitory;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using CKBS.Controllers.API.QRCodeV1.Security;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class TableController : Controller
    {
        private readonly DataContext _context;
        private readonly ITable _tabel;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IDataProtector _protector;
        private string ImagePath => Path.Combine(_appEnvironment.WebRootPath, "Images/table");
        public TableController(DataContext context,ITable table, IWebHostEnvironment hostingEnvironment,
            IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _context = context;
            _tabel = table;
            _appEnvironment = hostingEnvironment;
            _protector = dataProtectionProvider.CreateProtector(
               dataProtectionPurposeStrings.TableIDRouteValue);
        }

        [Privilege("A009")]
        public IActionResult Index()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.Administrator = "show";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            ViewData["GrouptableID"] = new SelectList(_context.GroupTables.Where(i => !i.Delete), "ID", "Name");
            return View();
        }

        [Privilege("A009")]
        public IActionResult QRcodeTable(int tableId)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.Administrator = "show";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            ViewBag.TableID =tableId.ToString();// _protector.Protect(tableId.ToString());
            var tablename = _context.Tables.Find(tableId) ?? new Table();
            ViewBag.TableName = tablename.Name;
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

        public IActionResult GetTable(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var table = from tab in _context.Tables.Where(t => t.Delete == false)
                        join grou in _context.GroupTables.Where(i => !i.Delete) on tab.GroupTableID equals grou.ID
                        select new
                        {
                            tab.ID,
                            tab.Name,
                            TableName = tab.Name,
                            GroupTableName = grou.Name,
                            tab.GroupTableID,
                            tab.Image,
                        };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                table = table.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(table.ToList());
        }

        [Privilege("A009")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            ViewData["GroupTableID"] = new SelectList(_context.GroupTables.Where(i => !i.Delete), "ID", "Name");
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i=> !i.Delete), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Table table)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            if (table.GroupTableID == 0)
            {
                ViewBag.required = "Please select group table !";
            }
           
            if (ModelState.IsValid)
            {
                await _tabel.AddOrEdit(table);
                UploadImg(table);
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupTableID"] = new SelectList(_context.GroupTables.Where(i => !i.Delete), "ID", "Name", table.GroupTableID);
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            return View(table);
        }

        private void UploadImg(Table table)
        {
            try
            {
                var Image = HttpContext.Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    var file = Image;
                    if (file.Length > 0)
                    {
                        using var fileStream = new FileStream(Path.Combine(ImagePath, table.Image), FileMode.Create);
                        file.CopyTo(fileStream);

                    }
                }
            }
            catch (Exception){}
        }

        [Privilege("A009")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            var table = _tabel.GetById(id);
            if (table.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (table == null)
            {
                return NotFound();
            }
            ViewData["GroupTableID"] = new SelectList(_context.GroupTables.Where(i => !i.Delete), "ID", "Name", table.GroupTableID);
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Table table)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Table";
            ViewBag.Subpage = "Table";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.TableMenu = "show";
            ViewBag.Table = "highlight";
            if (table.GroupTableID == 0)
            {
                ViewBag.required = "Please select group table !";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tabel.AddOrEdit(table);
                    UploadImg(table);
                }
                catch (Exception)
                {
                   
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupTableID"] = new SelectList(_context.GroupTables.Where(i => !i.Delete), "ID", "Name", table.GroupTableID);
            ViewBag.PriceLists = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            return View(table);
        }

       [HttpPost]
       public async Task<IActionResult> DeleteTable(int id)
        {
           await  _tabel.DeleteTable(id);
            return Ok();

        }
       
    }
}
