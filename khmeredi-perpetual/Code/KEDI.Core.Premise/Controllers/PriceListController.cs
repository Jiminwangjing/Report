using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Models.Validation;

namespace CKBS.Controllers
{
    [Privilege]
    public class PriceListController : Controller
    {
        private readonly DataContext _context;
        private readonly IPriceList _priceList;
        private static int PriceListID = 0;
        private static int PriceID = 0;
        public PriceListController(DataContext context, IPriceList priceList)
        {
            _context = context;
            _priceList = priceList;
        }

        public IActionResult CopyItem(int ID)
        {
            var pricelist = _context.PriceLists.FirstOrDefault(w => w.ID == ID && w.Delete == false);
            ViewBag.PriceListFromID = ID;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Copy Item To " + pricelist.Name;
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            return View();
        }

        public IActionResult SetSalePrice(int id)
        {
            var pricelist = _context.PriceLists.FirstOrDefault(w => w.ID == id && w.Delete == false);
            ViewBag.PriceListID = id;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            ViewBag.Subpage = "Set Price -> " + pricelist?.Name;
            return View();
        }

        public IActionResult UpdateSalePrice(int ID)
        {
            var pricelist = _context.PriceLists.FirstOrDefault(w => w.ID == ID && w.Delete == false);
            ViewBag.PriceListID = ID;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Update Price -> " + pricelist?.Name;
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            return View();
        }

        [HttpGet]
        [Privilege("A017")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Price list";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult GetPriceLists()
        {
            var data = _priceList.GetPriceLists().OrderByDescending(g => g.ID);
            return Ok(data);
        }
        [Privilege("A017")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Price list";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";

            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => !c.Delete), "ID", "Symbol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Delete,CurrencyID")] PriceLists priceList)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Price list";
            ViewBag.type = "Create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            if (ModelState.IsValid)
            {
                await _priceList.AddOrEdit(priceList);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (priceList.CurrencyID == 0)
                {
                    ViewBag.Error = "Please select currency !";
                    ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => !c.Delete), "ID", "Symbol", priceList.CurrencyID);
                }

            }
            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => !c.Delete), "ID", "Symbol", priceList.CurrencyID);
            return View(priceList);
        }

        [Privilege("A017")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Price list";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            var priceList = _priceList.GetId(id);
            if (priceList == null)
            {
                return NotFound();
            }
            if (priceList.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => !c.Delete), "ID", "Symbol", priceList.CurrencyID);
            return View(priceList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Delete,CurrencyID")] PriceLists priceList)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Price list";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            if (ModelState.IsValid)
            {
                try
                {
                    await _priceList.AddOrEdit(priceList);
                }
                catch (Exception)
                {
                    ViewBag.Error = "Please select currency !";
                    ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => !c.Delete), "ID", "Symbol", priceList.CurrencyID);
                    return View(priceList);
                }
                return RedirectToAction(nameof(Index));
            }

            return View(priceList);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            await _priceList.Deletepricelist(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InsertPricelist(PriceLists priceList)
        {
            await _priceList.AddOrEdit(priceList);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetselectPricelist()
        {
            var list = _priceList.GetPriceLists().OrderByDescending(p => p.ID).ToList();
            return Ok(list);
        }

        [Privilege("A017")]
        public IActionResult SetPrice(int ID)
        {
            PriceListID = ID;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Set Price";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetPricelistItem()
        {
            var list = _priceList.ItemMasters("A", PriceListID).ToList();
            return Ok(list);
        }

        [Privilege("A017")]
        public IActionResult Update(int ID)
        {
            PriceID = ID;
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Price List";
            ViewBag.Subpage = "Update Price";
            ViewBag.InventoryMenu = "show";
            ViewBag.PriceList = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetPriceListUpdate()
        {
            var list = _priceList.ItemMasters("M", PriceID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult UpdateSetPrice(ServiceDetail dataService)
        {
            _priceList.UpdatePriceListDetail(dataService);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetPriceListByGroup1(int ID, int PriceListID)
        {
            if (ID == 0)
            {
                var list = _priceList.ItemMasters("A", PriceListID).ToList();
                return Ok(list);
            }
            else
            {
                var list = _priceList.ItemMasters("A", PriceListID).Where(x => x.Group1 == ID).ToList();
                return Ok(list);
            }
        }

        [HttpGet]
        public IActionResult GetPriceListByGroup1_M(int ID, int PriceListID)
        {
            if (ID == 0)
            {
                var list = _priceList.ItemMasters("M", PriceListID).ToList();
                return Ok(list);
            }
            else
            {
                var list = _priceList.ItemMasters("M", PriceListID).Where(x => x.Group1 == ID).ToList();
                return Ok(list);
            }

        }

        [HttpGet]
        public IActionResult GetPriceListDetailByGroup2(int ID, int PriceListID)
        {
            var list = _priceList.ItemMasters("A", PriceListID).Where(x => x.Group2 == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPriceListDetailByGroup2_M(int ID, int PriceListID)
        {
            var list = _priceList.ItemMasters("M", PriceListID).Where(x => x.Group2 == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPriceListDetailByGroup3(int ID, int PriceListID)
        {
            var list = _priceList.ItemMasters("A", PriceListID).Where(x => x.Group3 == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPriceListDetailByGroup3_M(int ID, int PriceListID)
        {
            var list = _priceList.ItemMasters("M", PriceListID).Where(x => x.Group3 == ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetInventoryAuditByItem(int ItemID, int BranchID, int UomID)
        {
            var list = _priceList.GetInventoryAuditsByItem(ItemID, BranchID, UomID).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult FilterSystemDate(string DateFrom, string DateTo, int ItemID, int BranchID, int UomID)
        {
            var list = _priceList.GetInventoryAuditsFilterSystemDate(DateFrom, DateTo, ItemID, BranchID, UomID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehousefilter(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        //Panha
        public IActionResult GetPriceList(int price_list_baseid)
        {
            var pricelist = _context.PriceLists.Where(w => w.Delete == false && w.ID != price_list_baseid);
            return Ok(pricelist);
        }

        public IActionResult GetGroup1()
        {
            var groups = _context.ItemGroup1.Where(w => w.Delete == false);
            return Ok(groups);
        }

        public IActionResult GetGroup2(int group1)
        {
            var groups = _context.ItemGroup2.Where(w => w.ItemG1ID == group1 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }

        public IActionResult GetGroup3(int group1, int group2)
        {
            var groups = _context.ItemGroup3.Where(w => w.ItemG1ID == group1 && w.ItemG2ID == group2 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }

         public async Task<IActionResult> GetItemMasterToCopy(int pricelistbase, int pricelistid, int group1, int group2, int group3)
        {
            try
            {
                var items = await _priceList.GetItemMasterToCopy(pricelistbase, pricelistid, group1, group2, group3);
                
                return Ok(items);
            }
            catch
            {
                return Ok(new List<ServicePriceListCopyItem>());
            }
        }
        public IActionResult ItemCopyToPriceList(string ItemCopyToPriceList)
        {
            ItemCopyToPriceList itemcopypricelist = JsonConvert.DeserializeObject<ItemCopyToPriceList>(ItemCopyToPriceList, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            _priceList.InsertIntoPricelist(itemcopypricelist);
            return Ok('Y');
        }

        //12.03.19
        public IActionResult GetItemsSetPrice(int PriceListID, int Group1, int Group2, int Group3)
        {
            IEnumerable<PricelistSetPrice> items = _context.PricelistSetPrice.FromSql("sp_GetItemSetPrice @PriceListID={0},@Group1={1},@Group2={2},@Group3={3},@Process={4}",

              parameters: new[] {
                    PriceListID.ToString(),
                    Group1.ToString(),
                    Group2.ToString(),
                    Group3.ToString(),
                    "Add"
              }).ToList();
            foreach (var item in items)
            {
                var data = _context.PriceListDetails.FirstOrDefault(w => w.ID == item.ID);
                var list = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == data.ItemID);
                item.Barcode = list.Barcode;
            }
            return Ok(items);
        }

        public IActionResult GetItemsUpdatePrice(int PriceListID, int Group1, int Group2, int Group3)
        {
            IEnumerable<PricelistSetPrice> items = _context.PricelistSetPrice.FromSql("sp_GetItemSetPrice @PriceListID={0},@Group1={1},@Group2={2},@Group3={3},@Process={4}",
             parameters: new[] {

                    PriceListID.ToString(),
                    Group1.ToString(),
                    Group2.ToString(),
                    Group3.ToString(),
                    "Update"
             }).ToList();
            return Ok(items);
        }

        [HttpPost]
        public IActionResult SetAndUpdateSalePrice(string data)
        {
            PricelistSetUpdatePrice _data = JsonConvert.DeserializeObject<PricelistSetUpdatePrice>(data);
            if (_data.PricelistSetPrice == null) { return Ok('N'); }
            if (_data.PricelistSetPrice.Count > 0)
            {
                foreach (var item in _data.PricelistSetPrice)
                {
                    var item_update = _context.PriceListDetails.Find(item.ID);
                    var itemMaster = _context.ItemMasterDatas
                        .FirstOrDefault(i => i.ID == item_update.ItemID && Convert.ToInt32(i.InventoryUoMID) == item_update.UomID);
                    if (itemMaster != null)
                    {
                        itemMaster.Barcode = item.Barcode;
                        _context.SaveChanges();
                    }
                    item_update.Cost = item.Cost;
                    item_update.UnitPrice = item.Price;
                    item_update.Barcode = item.Barcode;
                    _context.PriceListDetails.Update(item_update);
                }
                _context.SaveChanges();
                return Ok('Y');
            }
            return Ok('N');
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePriceListDetial(PricelistSetPrice data)
        {
            ModelMessage msg = new();
            var pld = await _context.PriceListDetails.FindAsync(data.ID);
            if (pld == null)
            {
                ModelState.AddModelError("Error", "Could update the data");
                return Ok(msg.Bind(ModelState));
            }
            var itemMaster = _context.ItemMasterDatas
                        .FirstOrDefault(i => i.ID == pld.ItemID && Convert.ToInt32(i.InventoryUoMID) == pld.UomID);
            if (itemMaster != null)
            {
                itemMaster.Barcode = data.Barcode;
                _context.SaveChanges();
            }
            pld.Cost = data.Cost;
            pld.Barcode = data.Barcode;
            pld.UnitPrice = data.Price;
            _context.PriceListDetails.Update(pld);
            await _context.SaveChangesAsync();
            ModelState.AddModelError("Success", "Price List data Updated");
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }
        public JsonResult SetAndUpdateSalePrice1()
        {
            string json;
            using (var reader = new StreamReader(Request.Body))
            {
                json = reader.ReadToEnd();
            }
            PricelistSetUpdatePrice dataVM = JsonConvert.DeserializeObject<PricelistSetUpdatePrice>(json);
            if (dataVM.PricelistSetPrice.Count > 0)
            {
                foreach (var item in dataVM.PricelistSetPrice.ToList())
                {
                    var item_update = _context.PriceListDetails.Find(item.ID);
                    item_update.Cost = item.Cost;
                    item_update.UnitPrice = item.Price;
                    _context.PriceListDetails.Update(item_update);
                }
                _context.SaveChanges();
                return Json('Y');
            }
            return Json('Y');
        }


    }
}
