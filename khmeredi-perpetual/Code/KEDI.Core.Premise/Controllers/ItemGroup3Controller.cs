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
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers.Developer2
{
    [Privilege]
    public class ItemGroup3Controller : Controller
    {
        private readonly DataContext _context;
        public readonly IItemGroup3 _itemGroup3;
        private readonly IWebHostEnvironment _appEnvironment;
        private string _imagePath => Path.Combine(_appEnvironment.WebRootPath, "Images/items");
        public ItemGroup3Controller(DataContext context,IItemGroup3 itemGroup3, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _itemGroup3 = itemGroup3;
            _appEnvironment = hostingEnvironment;
        }

        [Privilege("A020")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(3)";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup3 = "highlight";
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

        public IActionResult GetItemGroups3(string keyword = "")
        {
            var itemGroup3 = from item3 in _context.ItemGroup3.Where(i3 => i3.Delete == false)
                             join item1 in _context.ItemGroup1.Where(i1 => i1.Delete == false) on item3.ItemG1ID equals item1.ItemG1ID
                             join item2 in _context.ItemGroup2.Where(i2 => i2.Delete == false) on item3.ItemG2ID equals item2.ItemG2ID
                             join col in _context.Colors.Where(c => c.Delete == false) on item3.ColorID equals col.ColorID
                             join bak in _context.Backgrounds.Where(b => b.Delete == false) on item3.BackID equals bak.BackID
                             where item3.Delete == false && item3.Name != "None"

                             select new
                             {
                                 ID = item3.ID,
                                 ItemG1ID = item3.ItemG1ID,
                                 ItemG2ID = item3.ItemG2ID,
                                 Name = item3.Name,
                                 ItemGroup1 = item1.Name,
                                 ItemGroup2 = item1.Name,
                                 Photo = item3.Images,
                                 ColorID = item3.ColorID,
                                 BackID = item3.BackID,
                                 Images = item3.Images,
                             };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                itemGroup3 = itemGroup3.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(itemGroup3.ToList());
        }

        [Privilege("A020")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(3)";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup3 = "highlight";
            ViewData["BackID"] = new SelectList(_context.Backgrounds, "BackID", "Name");
            ViewData["ColorID"] = new SelectList(_context.Colors, "ColorID", "Name");
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1, "ItemG1ID", "Name");
            ViewData["ItemG2ID"] = new SelectList(_context.ItemGroup2, "ItemG2ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Delete,Images,ItemG1ID,ItemG2ID,ColorID,BackID")] ItemGroup3 itemGroup3)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(3)";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup3 = "highlight";
            if (itemGroup3.ItemG1ID == 0)
            {
                ViewBag.Error = " Please select name !";

            }
            else if (itemGroup3.ItemG2ID == 0)
            {
                ViewBag.Error = "Please select  name !";
            }
            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        if (file.Length > 0)
                        {
                            using (var fileStream = new FileStream(Path.Combine(_imagePath, itemGroup3.Images), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                itemGroup3.Images = itemGroup3.Images;
                            }

                        }
                    }
                }

               await _itemGroup3.AddItemGroup3(itemGroup3);
               return RedirectToAction(nameof(Index));
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds, "BackID", "Name", itemGroup3.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors, "ColorID", "Name", itemGroup3.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1, "ItemG1ID", "Name", itemGroup3.ItemG1ID);
            ViewData["ItemG2ID"] = new SelectList(_context.ItemGroup2, "ItemG2ID", "Name", itemGroup3.ItemG2ID);
            return View(itemGroup3);
        }

        [Privilege("A020")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(3)";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup3 = "highlight";
            var itemGroup3 = _itemGroup3.GetByID(id);
            if (itemGroup3.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (itemGroup3 == null)
            {
                return NotFound();
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds, "BackID", "Name", itemGroup3.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors, "ColorID", "Name", itemGroup3.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1, "ItemG1ID", "Name", itemGroup3.ItemG1ID);
            ViewData["ItemG2ID"] = new SelectList(_context.ItemGroup2, "ItemG2ID", "Name", itemGroup3.ItemG2ID);
            return View(itemGroup3);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Delete,Images,ItemG1ID,ItemG2ID,ColorID,BackID")] ItemGroup3 itemGroup3)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(3)";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup3 = "highlight";

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        if (file.Length > 0)
                        {
                            using (var fileStream = new FileStream(Path.Combine(_imagePath, itemGroup3.Images), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                itemGroup3.Images = itemGroup3.Images;
                            }

                        }
                    }
                }
                await _itemGroup3.AddItemGroup3(itemGroup3);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds, "BackID", "Name", itemGroup3.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors, "ColorID", "Name", itemGroup3.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1, "ItemG1ID", "Name", itemGroup3.ItemG1ID);
            ViewData["ItemG2ID"] = new SelectList(_context.ItemGroup2, "ItemG2ID", "Name", itemGroup3.ItemG2ID);
            return View(itemGroup3);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCateory(int ID)
        {
            await _itemGroup3.DeleteItemGroup3(ID);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddItemGroup3(ItemGroup3 itemGroup3)
        {
            var list = await  _itemGroup3.AddItemGroup3(itemGroup3);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetitemGroup3(int Item2ID,int Item1ID)
        {
            var list = _itemGroup3.ItemGroup3s().OrderByDescending(c=>c.ID).Where(x => x.ItemG2ID == Item2ID  && x.ItemG1ID== Item1ID).ToList();
            return Ok(list);
        }
    }
}
