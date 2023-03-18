using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class WarehouseController : Controller
    {
        private readonly IWarehouse _context;
        private readonly DataContext _dataContext;
      
        public WarehouseController(IWarehouse context,DataContext dataContext)
        {
            _context = context;
            _dataContext = dataContext;
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int _userId);
            return _userId;
        }

        public IActionResult CopyItem(int ID)
        {
            var warehouse = _context.Warehouse.FirstOrDefault(w => w.ID == ID && w.Delete == false);
            ViewBag.WHFromID = ID;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "From Warehouse" + warehouse.Name;
            ViewBag.Administrator = "show";
            ViewBag.Inventory = "show";
            ViewBag.Warehouse = "highlight";
            return View();
        }

        [Privilege("A008")]
        public IActionResult Index()
        {
            ViewBag.Warehouse = "highlight";
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

        public IActionResult GetWarehouses(string keyword = "")
        {
            var warehouse = (from w in _context.Warehouse.Where(x => x.Delete == false)
                             join b in _dataContext.Branches.Where(x => x.Delete == false) on w.BranchID equals b.ID
                             //let whS = _dataContext.WarehouseSummary.Where(whs => w.ID == whs.WarehouseID).ToList()
                             select new
                             {
                                 w.ID,
                                 w.Code,
                                 w.Name,
                                 Branch = b.Name,
                                 w.BranchID,
                                 w.StockIn,// = whS.Sum(i=> i.InStock),
                                 w.Location,
                                 w.Address
                             }).ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                warehouse = warehouse.Where(c => RawWord(c.Code).Contains(keyword, ignoreCase)).ToList();
            }
            return Ok(warehouse.ToList());
        }

        [Privilege("A008")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Warehouse";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Inventory = "show";
            ViewBag.Warehouse = "highlight";
            ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Warehouse";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Inventory = "show";
            ViewBag.Warehouse = "highlight";
            if (warehouse.BranchID == 0)
            {
                ViewBag.requaridBranch = "Pleas select branch !";
                ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name", warehouse.BranchID);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    
                    await _context.AddOrEdit(warehouse);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {            
                    ViewBag.ErrorCode = "This code already exist !";
                    ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name", warehouse.BranchID);
                    return View(warehouse);
                }
            }
            ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name",warehouse.BranchID);
            return View(warehouse);
        }

        // GET: Warehouse/Edit/5
        [Privilege("A008")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Warehouse";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Inventory = "show";
            ViewBag.Warehouse = "highlight";
            if (ModelState.IsValid)
            {
                var warehouse = _context.GetId(id);
                if (warehouse.Delete == true)
                {
                    return RedirectToAction(nameof(Index));
                }
                ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name");
                return View(warehouse);
            }
            return Ok();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Warehouse warehouse)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Warehouse";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Inventory = "show";
            ViewBag.Warehouse = "highlight";
            if (warehouse.BranchID == 0)
            {
                ViewBag.requaridBranch = "Pleas select branch !";
                ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name", warehouse.BranchID);
            }

            if (id != warehouse.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.AddOrEdit(warehouse);
                }
                catch (Exception)
                {
                    ViewBag.ErrorCode = "This code already exist !";
                    return View(warehouse);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchID"] = new SelectList(_dataContext.Branches.Where(x => x.Delete == false), "ID", "Name",warehouse.BranchID);
            return View(warehouse);
        }

        // POST: Warehouse/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _context.Delete(id);
            return Ok();
        }

        // POST: Warehouse/SetDefault/5
        [HttpPost]
        public async Task<IActionResult> SetDefault(int id)
        {
            if(ModelState.IsValid)
            {
                await _context.SetDefault(id);
                return RedirectToAction(nameof(Index));
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult AddWarehouse(Warehouse warehouse)
        {                   
             var list= _context.AddOrEdit(warehouse);
             return Ok(list);         
        }

        [HttpGet]
        public IActionResult GetWarehouse()
        {
            var list = _context.Warehouses().Where(w => w.Delete == false).ToList();
            return Ok(list);

        }

        [HttpGet]
        public IActionResult CheckCodeWarehouse(string Code)
        {
            var list = _context.Warehouse.Where(x => x.Code == Code);
            return Ok(list);
        }

        //Panha
        public IActionResult GetWarehouseTo()
        {
            var warehouse = _dataContext.Warehouses.Where(w => w.Delete == false);
            return Ok(warehouse);
        }

        public IActionResult GetGroup1()
        {
            var groups = _dataContext.ItemGroup1.Where(w => w.Delete == false);
            return Ok(groups);
        }

        public IActionResult GetGroup2(int group1)
        {
            var groups = _dataContext.ItemGroup2.Where(w => w.ItemG1ID == group1 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }

        public IActionResult GetGroup3(int group1, int group2)
        {
            var groups = _dataContext.ItemGroup3.Where(w => w.ItemG1ID == group1 && w.ItemG2ID == group2 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }

        public IActionResult GetItemMasterToCopy(int from_whid,int to_whid, int group1, int group2, int group3)
        {
            var items = _context.GetItemMasterToCopy(from_whid,to_whid, group1, group2, group3).ToList();
            return Ok(items);
        }

        public IActionResult ItemCopyToWH(ItemCopyToWH ItemCopyToWH)
        {
            _context.InsertIntoPricelist(ItemCopyToWH, GetUserID());
            return Ok('Y');
        }
    }
}
