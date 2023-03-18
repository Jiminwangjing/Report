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
    public class ItemGroup2Controller : Controller
    {
        private readonly DataContext _context;
        private readonly IItem2Group _item2Group;
        private readonly IWebHostEnvironment _appEnvironment;
        private string ImagePath => Path.Combine(_appEnvironment.WebRootPath, "Images/items");
        public ItemGroup2Controller(DataContext context,IItem2Group item2Group, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _item2Group = item2Group;
            _appEnvironment = hostingEnvironment;  
        }

        // GET: ItemGroup2
        [Privilege("A019")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(2)";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup2 = "highlight";
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

        public IActionResult GetItemGroups2(string keyword = "")
        {
            var itemGroup2 = from item1 in _context.ItemGroup1.Where(i1 => i1.Delete == false)
                             join item2 in _context.ItemGroup2.Where(i2 => i2.Delete == false) on item1.ItemG1ID equals item2.ItemG1ID
                             join col in _context.Colors.Where(c => c.Delete == false) on item2.ColorID equals col.ColorID
                             join bak in _context.Backgrounds.Where(b => b.Delete == false) on item2.BackID equals bak.BackID
                             where item2.Delete == false && item2.Name != "None"
                             select new
                             {
                                 item2.ItemG1ID,
                                 item2.ItemG2ID,
                                 item2.Name,
                                 ItemGroup1 = item1.Name,
                                 Photo = item2.Images,
                                 item2.ColorID,
                                 item2.BackID,
                                 item2.Images,

                             };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                itemGroup2 = itemGroup2.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(itemGroup2.ToList());

        }

        [Privilege("A019")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(2)";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup2 = "highlight";
            ViewData["BackID"] = new SelectList(_context.Backgrounds.Where(c => c.Delete == false), "BackID", "Name");
            ViewData["ColorID"] = new SelectList(_context.Colors.Where(b => b.Delete == false), "ColorID", "Name");
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1.Where(i => i.Delete == false), "ItemG1ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemG2ID,Name,Images,Delete,ItemG1ID,ColorID,BackID")] ItemGroup2 itemGroup2)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(2)";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup2 = "highlight";
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
                            using var fileStream = new FileStream(Path.Combine(ImagePath, itemGroup2.Images), FileMode.Create);
                            await file.CopyToAsync(fileStream);
                            itemGroup2.Images = itemGroup2.Images;

                        }
                    }
                }
                if (itemGroup2.ItemG1ID == 0)
                {
                    ViewBag.Error = "Please select Item1 Name!";
                    
                }
                else
                {

                    await _item2Group.AddItemGroup2(itemGroup2);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds.Where(c => c.Delete == false), "BackID", "Name", itemGroup2.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors.Where(c => c.Delete == false), "ColorID", "Name", itemGroup2.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1.Where(c => c.Delete == false), "ItemG1ID", "Name", itemGroup2.ItemG1ID);
            return View(itemGroup2);
        }

        [Privilege("A019")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(2)";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup2 = "highlight";
            var itemGroup2 = _item2Group.getid(id);
            if (itemGroup2.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (itemGroup2 == null)
            {
                return NotFound();
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds.Where(c => c.Delete == false), "BackID", "Name", itemGroup2.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors.Where(c => c.Delete == false), "ColorID", "Name", itemGroup2.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1.Where(c => c.Delete == false), "ItemG1ID", "Name", itemGroup2.ItemG1ID);
            return View(itemGroup2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ItemG2ID,Name,Images,Delete,ItemG1ID,ColorID,BackID")] ItemGroup2 itemGroup2)
        {

            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(2)";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup2 = "highlight";
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
                            //var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using var fileStream = new FileStream(Path.Combine(ImagePath, itemGroup2.Images), FileMode.Create);
                            await file.CopyToAsync(fileStream);
                            itemGroup2.Images = itemGroup2.Images;

                        }
                    }
                }

                if (itemGroup2.ItemG1ID <= 0)
                {
                    ViewBag.Error = "Please select Item1 Name!";

                }
                else
                {
                    await _item2Group.AddItemGroup2(itemGroup2);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BackID"] = new SelectList(_context.Backgrounds.Where(c => c.Delete == false), "BackID", "Name", itemGroup2.BackID);
            ViewData["ColorID"] = new SelectList(_context.Colors.Where(c => c.Delete == false), "ColorID", "Name", itemGroup2.ColorID);
            ViewData["ItemG1ID"] = new SelectList(_context.ItemGroup1.Where(c => c.Delete == false), "ItemG1ID", "Name", itemGroup2.ItemG1ID);
            return View(itemGroup2);
        }

       [HttpGet]
       public IActionResult GetColor(int id)
        {
            var list = _item2Group.GetColors.Where(c => c.ColorID == id);
            return Ok(list);

        }

        [HttpGet]
        public IActionResult GetBackground(int id)
        {
            var list = _item2Group.GetBackgrounds.Where(b => b.BackID == id);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCateory(int ID)
        {
             await _item2Group.DeleteItemGroup2(ID);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetItemGroup2(int Item1ID)
        {
            var list= _item2Group.ItemGroup2s().Where(c => c.ItemG1ID == Item1ID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> QuickAdd(ItemGroup2 itemGroup2)
        {        
            await  _item2Group.AddItemGroup2(itemGroup2);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetData(int ID)
        {
            var list = _item2Group.ItemGroup2s().OrderByDescending(x=>x.ItemG2ID).Where(c=>c.ItemG1ID==ID);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Additmegroup2(ItemGroup2 itemGroup2)
        {
            var list = await _item2Group.AddItemGroup2(itemGroup2);
            return Ok(list);
        }
    }
}
