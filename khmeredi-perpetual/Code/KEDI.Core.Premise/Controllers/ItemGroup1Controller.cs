using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.Financials;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using KEDI.Core.System.Models;

namespace CKBS.Controllers.Developer2
{
    [Privilege]
    public class ItemGroup1Controller : Controller
    {
        private readonly DataContext _context;
        private readonly IItemGroup _itemGroup;
        private readonly IWebHostEnvironment _appEnvironment;
        private string ImagePath => Path.Combine(_appEnvironment.WebRootPath, "Images/items");

        readonly string[] ___item = {
                // "Expense Account *",
                // "Revenue Account *",
                // "Inventory Account *",
                // "Cost of Goods Sold Account *",
                // "Allocation Account *",
                "Expense Account",
                "Revenue Account",
                "Inventory Account",
                "Cost of Goods Sold Account",
                "Allocation Account",
                "Variance Account",
                "Price Difference Account",
                "Negative Inventory Adjustment Acct",
                "Inventory Offset Decrease Account",
                "Inventory Offset Increase Account",
                "Sales Returns Account",
                "Revenue Account EU",
                "Expense Account EU",
                "Revenue Account Foreign",
                "Expense Account Foreign",
                "Exchange Rate Differences Account",
                "Goods Clearing Account",
                "GL Decrease Account",
                "GL Increase Account",
                "WIP Inventory Account",
                "WIP Inventory Variance Account",
                "WIP Offset PL Account",
                "Inventory Offset PL Account",
                "Expense Clearing Account",
                "Stock In Transit Account",
                "Shipped Goods Account",
                "Sales Credit Account",
                "Purchase Credit Account",
                "Sales Credit Account Foreign",
                "Purchase Credit Account Foreign",
                "Sales Credit Account EU",
                "Purchase Credit Account EU"
            };
        public ItemGroup1Controller(DataContext context, IItemGroup itemGroup, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _itemGroup = itemGroup;
            _appEnvironment = hostingEnvironment;

        }
        // accounting name group
        public IActionResult Getname()
        {
            return Ok(___item);
        }

        //GET: GlAccount with Active Account

        public IActionResult GetGlAccountLastLevel()
        {
            var glAcc = _context.GLAccounts.Where(i => i.IsActive == true).OrderBy(i => i.Code).ToList();
            return Ok(glAcc);
        }

        // GET: ItemGroup1
        [Privilege("A018")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(1)";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup1 = "highlight";

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

        public IActionResult GetItemGroup1(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var itemGroup1 = from s in _context.ItemGroup1.Where(cate => cate.Delete == false)
                             join c in _context.Colors.Where(col => col.Delete == false) on s.ColorID equals c.ColorID
                             join bak in _context.Backgrounds.Where(b => b.Delete == false) on s.BackID equals bak.BackID
                             where s.Delete == false

                             select new
                             {
                                 s.ItemG1ID,
                                 s.Name,
                                 Photo = s.Images,
                                 s.Images

                             };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                itemGroup1 = itemGroup1.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(itemGroup1.ToList());
        }

        [Privilege("A018")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "ItemGroup";
            ViewBag.Subpage = "Item Group(1)";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup1 = "highlight";
            ViewData["Name"] = new SelectList(_context.Colors, "ColorID", "Name");
            ViewData["Name"] = new SelectList(_context.Backgrounds, "BackID", "Name");
            return View(new ItemAccountingView
            {
                ItemGroup1 = new ItemGroup1(),
                ItemAccounting = new ItemAccounting(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageCreate(IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var checkImg = _context.ItemGroup1.LastOrDefault(s => s.Images == image.FileName);
                    var file = image;
                    var itemGroup1Id = checkImg.ItemG1ID;
                    var itemGroup1 = _context.ItemGroup1.Find(itemGroup1Id);
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        itemGroup1.Images = fileName;
                        using var fileStream = new FileStream(Path.Combine(ImagePath, itemGroup1.Images), FileMode.Create);
                        await file.CopyToAsync(fileStream);
                        _context.SaveChangesAsync().Wait();
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageEdit(IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var file = image;
                    var itemGroup1Id = int.Parse(Request.Form["ItemGroup1Id"].ToString());
                    var itemGroup1 = _context.ItemGroup1.Find(itemGroup1Id);
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        itemGroup1.Images = fileName;
                        using var fileStream = new FileStream(Path.Combine(ImagePath, itemGroup1.Images), FileMode.Create);
                        await file.CopyToAsync(fileStream);
                        _context.SaveChangesAsync().Wait();
                    }

                }
            }
            return Ok();
        }

        public IActionResult GetGlProp()
        {
            List<Prop> prop = new();
            foreach (var (value, index) in ___item.Select((v, i) => (v, i)))
            {
                prop.Add(new Prop
                {
                    LineID = index,
                    NameProp = value,
                });
            }
            return Ok(prop);
        }
        [HttpPost]
        public async Task<IActionResult> Create(string itemGroup1, string itemAccounting)
        {
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup1 = "highlight";
            ItemGroup1 _itemGroup1 = JsonConvert.DeserializeObject<ItemGroup1>(itemGroup1,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            ItemAccounting _itemAccounting = JsonConvert.DeserializeObject<ItemAccounting>(itemAccounting,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            if (_itemGroup1.Name == "")
            {
                ModelState.AddModelError("Name", "Branch not matched with warehouse.");
            }

            if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                if (
                    string.IsNullOrEmpty(_itemGroup1.Name) || string.IsNullOrEmpty(_itemAccounting.RevenueAccount)
                    || string.IsNullOrEmpty(_itemAccounting.InventoryAccount) || string.IsNullOrEmpty(_itemAccounting.CostofGoodsSoldAccount)
                    || string.IsNullOrEmpty(_itemAccounting.AllocationAccount) || string.IsNullOrEmpty(_itemAccounting.ExpenseAccount)
                )
                {
                    return View(_itemGroup1);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        await _itemGroup.AddItemGroup(_itemGroup1);
                        var itemGroupID = _itemGroup1.ItemG1ID;
                        _itemAccounting.ItemGroupID = itemGroupID;
                        _context.ItemAccountings.Update(_itemAccounting);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    await _itemGroup.AddItemGroup(_itemGroup1);
                    var itemGroupID = _itemGroup1.ItemG1ID;
                    _itemAccounting.ItemGroupID = itemGroupID;
                    _context.ItemAccountings.Update(_itemAccounting);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }


            return View(_itemGroup1);
        }

        [Privilege("A018")]
        public IActionResult Edit(int id)
        {
            ViewBag.ItemGroup = "show";
            ViewBag.ItemGroup1 = "highlight";

            var details = _context.ItemAccountings.Where(i => i.ItemGroupID == id).ToList().FirstOrDefault();
            ViewData["Name"] = new SelectList(_context.Colors, "ColorID", "Name");
            ViewData["Name"] = new SelectList(_context.Backgrounds, "BackID", "Name");
            var itemGroup1 = _itemGroup.GetbyID(id);
            if (itemGroup1.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (itemGroup1 == null)
            {
                return NotFound();
            }
            return View(new ItemAccountingView
            {
                ItemGroup1 = itemGroup1,
                ItemAccounting = details ?? new ItemAccounting(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string itemGroup1, string itemAccounting)
        {
            ViewBag.button = "fa-edit";
            ViewBag.ItemGroup = "show";
            ItemGroup1 _itemGroup1 = JsonConvert.DeserializeObject<ItemGroup1>(itemGroup1,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            ItemAccounting _itemAccounting = JsonConvert.DeserializeObject<ItemAccounting>(itemAccounting,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                if (
                    string.IsNullOrEmpty(_itemGroup1.Name) || string.IsNullOrEmpty(_itemAccounting.RevenueAccount)
                    || string.IsNullOrEmpty(_itemAccounting.InventoryAccount) || string.IsNullOrEmpty(_itemAccounting.CostofGoodsSoldAccount)
                    || string.IsNullOrEmpty(_itemAccounting.AllocationAccount) || string.IsNullOrEmpty(_itemAccounting.ExpenseAccount)
                )
                {
                    return View(_itemGroup1);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        await _itemGroup.AddItemGroup(_itemGroup1);
                        var itemGroupID = _itemGroup1.ItemG1ID;
                        _itemAccounting.ItemGroupID = itemGroupID;
                        _context.ItemAccountings.Update(_itemAccounting);
                        _context.SaveChanges();
                        return Ok(_itemGroup1);
                    }
                    return View(itemGroup1);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    await _itemGroup.AddItemGroup(_itemGroup1);
                    var itemGroupID = _itemGroup1.ItemG1ID;
                    _itemAccounting.ItemGroupID = itemGroupID;
                    _context.ItemAccountings.Update(_itemAccounting);
                    _context.SaveChanges();
                    return Ok(_itemGroup1);
                }
                return View(itemGroup1);
            }
        }

        [HttpGet]
        public IActionResult GetColor(int id)
        {
            var list = _itemGroup.GetColors.Where(c => c.ColorID == id);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBackground(int id)
        {
            var list = _itemGroup.Backgrounds.Where(c => c.BackID == id);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCateory(int id)
        {
            await _itemGroup.DeleteItemGroup(id);
            return Ok();
        }

        [HttpGet]
        public IActionResult SelecrColor()
        {
            var list = _itemGroup.GetColors.OrderByDescending(x => x.ColorID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult SelectBackground()
        {
            var list = _itemGroup.Backgrounds.OrderByDescending(x => x.BackID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreate(ItemGroup1 itemGroup1)
        {
            await _itemGroup.AddItemGroup(itemGroup1);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetItemGroup()
        {
            var list = _itemGroup.GetItemGroup1s().OrderByDescending(x => x.ItemG1ID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Additmegroup1(ItemGroup1 itemGroup1)
        {
            var list = await _itemGroup.AddItemGroup(itemGroup1);
            return Ok(list);
        }

        [HttpPost]
        public IActionResult AddColors(Colors colors)
        {
            _context.Colors.Add(colors);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult AddBackground(Background background)
        {
            _context.Backgrounds.Add(background);
            _context.SaveChanges();
            return Ok();
        }
    }

    partial class Prop
    {
        public int LineID { get; set; }
        public string NameProp { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
