using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Page;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using System.Security.Cryptography;
using CKBS.Models.Services.ExcelFile;
using System.Text.RegularExpressions;
using CKBS.Models.Services.Financials;
using SetGlAccountMaster = CKBS.Models.Services.Inventory.SetGlAccount;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using CKBS.Models.ServicesClass.ItemMasterDataView;
using KEDI.Core.Localization.Resources;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Utilities;

namespace CKBS.Controllers
{
    [Privilege]
    public class ItemMasterDataController : Controller
    {
        private readonly DataContext _context;
        private readonly IItemMasterData _itemMasterData;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly CultureLocalizer _culLocal;
        private readonly WorkbookContext _workbook;
        private readonly WorkbookAdapter _wbExport;
        private readonly IGUOM _guom;
        private readonly UserManager _userModule;

        private string GetImagePath()
        {
            return Path.Combine(_appEnvironment.WebRootPath, "Images/items");
        }

        public ItemMasterDataController(DataContext context, IItemMasterData itemMasterData, IGUOM guom, UserManager userModule,
            IWebHostEnvironment hostingEnvironment, CultureLocalizer cultureLocalizer)
        {
            _context = context;
            _itemMasterData = itemMasterData;
            _guom = guom;
            _userModule = userModule;
            _appEnvironment = hostingEnvironment;
            _culLocal = cultureLocalizer;
            _workbook = new WorkbookContext();
            _wbExport = new WorkbookAdapter();
        }

        public IActionResult SetCurrentPage(string currPage)
        {
            ViewBag.PageRemeber = currPage;
            return Ok();
        }

        private async Task SetViewBagsAsync(ItemMasterData itemMasterData = null)
        {
            itemMasterData = itemMasterData ?? new ItemMasterData();
            int printerId = itemMasterData.PrintToID == null ? 0 : (int)itemMasterData.PrintToID;
            ViewData["GroupUomID"] = await _guom.SelectGroupDefinedUoMsAsync(itemMasterData.GroupUomID);
            ViewData["ItemGroup1ID"] = await _itemMasterData.SelectItemGroup1sAsync();
            ViewData["ItemGroup2ID"] = new SelectList(_context.ItemGroup2.Where(d => !d.Delete && d.ItemG1ID == itemMasterData.ItemGroup1ID && d.Name != "None"), "ItemG2ID", "Name");
            ViewData["ItemGroup3ID"] = new SelectList(_context.ItemGroup3.Where(d => !d.Delete && d.ItemG1ID == itemMasterData.ItemGroup1ID
                && d.ItemG2ID == itemMasterData.ItemGroup2ID && d.Name != "None"), "ID", "Name");
            ViewData["PriceListID"] = await _itemMasterData.SelectEntitiesAsync<PriceLists>(itemMasterData.PriceListID);
            ViewData["InventoryUoMID"] = await _guom.SelectDefinedUoMsAsync(itemMasterData.GroupUomID);
            ViewData["SaleUomID"] = await _guom.SelectDefinedUoMsAsync(itemMasterData.GroupUomID);
            ViewData["PurchaseUomID"] = await _guom.SelectDefinedUoMsAsync(itemMasterData.GroupUomID);
            ViewData["PrintToID"] = await _itemMasterData.SelectEntitiesAsync<PrinterName>(printerId);
            ViewData["WarehouseID"] = await _itemMasterData.SelectWarehousesAsync(itemMasterData.WarehouseID);
            ViewData["TagGroupPur"] = await _itemMasterData.TaxGroupView().Where(i => i.Type == (int)TypeTax.InputTax).ToListAsync();
            ViewData["TagGroupSales"] = await _itemMasterData.TaxGroupView().Where(i => i.Type == (int)TypeTax.OutPutTax).ToListAsync();
        }

        [Privilege("A016")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data ";
            ViewBag.Subpage = "List";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            if (Pagination.Page == "1/")
            {
                ViewBag.PageRemeber = "1/10/0";
            }
            else
            {
                ViewBag.PageRemeber = Pagination.Page;
            }
            //var data = _itemMasterData.GetItemMasterData(false, GetCompany().ID).OrderBy(g => g.ItemMasterData.Code);
            return View();
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID")?.Value, out int _id);
            return _id;
        }

        private int GetCompanyID()
        {
            var user = _context.UserAccounts.FirstOrDefault(u => u.ID == GetUserID());
            return (int)user?.CompanyID;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        public IActionResult IndexGrid(string SearchString = null)
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data ";
            ViewBag.Subpage = "Grid";
            ViewBag.Menu = "show";
            ViewData["CurrentFilter"] = SearchString;

            var data = _itemMasterData.GetMasterDatas();
            var Cate = from s in data select s;
            if (!string.IsNullOrEmpty(SearchString))
            {
                Cate = Cate.Where(s => s.Code.Contains(SearchString) || s.KhmerName.Contains(SearchString) || s.EnglishName.Contains(SearchString));
            }
            return View(Cate);
        }

        //public IActionResult GetItems(bool inActive)
        //{
        //    //Query all items
        //    var items = _itemMasterData.GetItemMasterData(inActive, GetCompanyID()).ToList();
        //    return Ok(items);
        //}

        public IActionResult GetItems(bool inActive, string keyword = "")
        {
            //Query all items
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Ok(SearchItems(inActive, keyword));
            }
            var items = _itemMasterData.GetItemMasterData(inActive, GetCompanyID());
            return Ok(items);
        }

        public List<ItemMasterDataViewModel> SearchItems(bool inActive, string keyword)
        {
            keyword = Regex.Replace(keyword, "\\s+", string.Empty, RegexOptions.IgnoreCase);
            StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            var itemMasters = _itemMasterData.GetItemMasterData(inActive, GetCompanyID());
            itemMasters = itemMasters.Where(m => m.ItemMasterData.Barcode.Contains(keyword, ignoreCase)
                || m.ItemMasterData.Code.Contains(keyword, ignoreCase)
                || m.ItemMasterData.KhmerName.Contains(keyword, ignoreCase)
                || m.ItemMasterData.EnglishName.Contains(keyword, ignoreCase)
                || m.ItemMasterData.ItemGroup1.Name.Contains(keyword, ignoreCase)
                || m.ItemMasterData.UnitofMeasureInv.Name.Contains(keyword, ignoreCase)).ToList();
            return itemMasters;
        }

        [HttpGet]
        public IActionResult GetMaster()
        {

            var list = _itemMasterData.GetMasterDatas().ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemGroupI()
        {
            var list = _context.ItemGroup1.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouseAccounting()
        {
            var wh = _context.Warehouses.Where(e => !e.Delete).ToList();
            List<WarehouseAccounting> whAcc = new();
            foreach (var (value, index) in wh.Select((v, i) => (v, i)))
            {
                whAcc.Add(new WarehouseAccounting
                {
                    LineID = index,
                    CodeWarehouse = value.Code,
                    WarehouseID = value.ID,
                    Name = value.Name,
                    Available = 0,
                    ItemGroupID = 0,
                    ItemID = 0,
                    MaximunInventory = 0,
                    MinimunInventory = 0,
                    Ordered = 0,
                    Committed = 0,
                    InStock = 0,
                    ID = 0,
                    SetGlAccount = 0,
                    AllocationAccount = "",
                    Code = "",
                    CostofGoodsSoldAccount = "",
                    ExchangeRateDifferencesAccount = "",
                    ExpenseAccount = "",
                    ExpenseAccountEU = "",
                    ExpenseAccountForeign = "",
                    ExpenseClearingAccount = "",
                    GLDecreaseAccount = "",
                    GLIncreaseAccount = "",
                    GoodsClearingAccount = "",
                    InventoryAccount = "",
                    InventoryOffsetDecreaseAccount = "",
                    InventoryOffsetIncreaseAccount = "",
                    InventoryOffsetPLAccount = "",
                    NegativeInventoryAdjustmentAcct = "",
                    PriceDifferenceAccount = "",
                    PurchaseCreditAccount = "",
                    PurchaseCreditAccountEU = "",
                    PurchaseCreditAccountForeign = "",
                    RevenueAccount = "",
                    RevenueAccountEU = "",
                    RevenueAccountForeign = "",
                    SalesCreditAccount = "",
                    SalesCreditAccountEU = "",
                    SalesCreditAccountForeign = "",
                    SalesReturnsAccount = "",
                    ShippedGoodsAccount = "",
                    StockInTransitAccount = "",
                    Type = "",
                    VarianceAccount = "",
                    WIPInventoryAccount = "",
                    WIPInventoryVarianceAccount = "",
                    WIPOffsetPLAccount = ""
                });
            }
            return Ok(whAcc);
        }
        public IActionResult WarehouseAccounting(int id)
        {
            //var wh = _context.Warehouses.Where(e => !e.Delete).ToList();
            var details = _context.ItemAccountings.AsNoTracking().Where(i => i.ItemID == id).Include(e => e.Warehouse).ToList();
            List<WarehouseAccounting> whAcc = new();
            if (details.Count > 0)
            {
                foreach (var (value, index) in details.Select((v, i) => (v, i)))
                {
                    whAcc.Add(new WarehouseAccounting
                    {
                        LineID = index,
                        CodeWarehouse = value.Warehouse.Code,
                        WarehouseID = value.Warehouse.ID,
                        Name = value.Warehouse.Name,
                        Available = value.Available,
                        ItemGroupID = value.ItemGroupID,
                        ItemID = value.ItemID,
                        MaximunInventory = value.MaximunInventory,
                        MinimunInventory = value.MinimunInventory,
                        Ordered = value.Ordered,
                        Committed = value.Committed,
                        InStock = value.InStock,
                        ID = value.ID,
                        SetGlAccount = value.SetGlAccount,
                        AllocationAccount = value.AllocationAccount,
                        Code = value.Code,
                        CostofGoodsSoldAccount = value.CostofGoodsSoldAccount,
                        ExchangeRateDifferencesAccount = value.ExchangeRateDifferencesAccount,
                        ExpenseAccount = value.ExpenseAccount,
                        ExpenseAccountEU = value.ExpenseAccountEU,
                        ExpenseAccountForeign = value.ExpenseAccountForeign,
                        ExpenseClearingAccount = value.ExpenseClearingAccount,
                        GLDecreaseAccount = value.GLDecreaseAccount,
                        GLIncreaseAccount = value.GLIncreaseAccount,
                        GoodsClearingAccount = value.GoodsClearingAccount,
                        InventoryAccount = value.InventoryAccount,
                        InventoryOffsetDecreaseAccount = value.InventoryOffsetDecreaseAccount,
                        InventoryOffsetIncreaseAccount = value.InventoryOffsetIncreaseAccount,
                        InventoryOffsetPLAccount = value.InventoryOffsetPLAccount,
                        NegativeInventoryAdjustmentAcct = value.NegativeInventoryAdjustmentAcct,
                        PriceDifferenceAccount = value.PriceDifferenceAccount,
                        PurchaseCreditAccount = value.PurchaseCreditAccount,
                        PurchaseCreditAccountEU = value.PurchaseCreditAccountEU,
                        PurchaseCreditAccountForeign = value.PurchaseCreditAccountForeign,
                        RevenueAccount = value.RevenueAccount,
                        RevenueAccountEU = value.RevenueAccountEU,
                        RevenueAccountForeign = value.RevenueAccountForeign,
                        SalesCreditAccount = value.SalesCreditAccount,
                        SalesCreditAccountEU = value.SalesCreditAccountEU,
                        SalesCreditAccountForeign = value.SalesCreditAccountForeign,
                        SalesReturnsAccount = value.SalesReturnsAccount,
                        ShippedGoodsAccount = value.ShippedGoodsAccount,
                        StockInTransitAccount = value.StockInTransitAccount,
                        Type = value.Type,
                        VarianceAccount = value.VarianceAccount,
                        WIPInventoryAccount = value.WIPInventoryAccount,
                        WIPInventoryVarianceAccount = value.WIPInventoryVarianceAccount,
                        WIPOffsetPLAccount = value.WIPOffsetPLAccount
                    });
                }
                return Ok(whAcc);
            }
            return GetWarehouseAccounting();
        }

        [HttpGet]
        public IActionResult GetItemGroupII(int ID)
        {
            var list = _context.ItemGroup2.Where(x => x.Delete == false && x.ItemG1ID == ID && x.Name != "None").ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemGroupIII(int ID, int Group1ID)
        {
            var list = _context.ItemGroup3.Where(x => x.Delete == false && x.ItemG1ID == Group1ID && x.ItemG2ID == ID && x.Name != "None").ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemMasterByGroup1(int ID)
        {
            if (ID == 0)
            {
                var data = _itemMasterData.GetMasterDatas().Where(x => x.Delete == false).ToList();
                return Ok(data);
            }
            else
            {
                var list = _itemMasterData.GetMasterDatasByCategory(ID);
                return Ok(list);
            }
        }
        [HttpGet]
        public IActionResult GetItemMasterByGroup2(int ID)
        {
            var list = _itemMasterData.GetMasterDatas().Where(x => x.ItemGroup2ID == ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemMasterByGroup3(int ID)
        {
            var list = _itemMasterData.GetMasterDatas().Where(x => x.ItemGroup3ID == ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult DetailItemMasterData(int ID)
        {
            var list = _itemMasterData.GetMasterDatas().Where(x => x.ID == ID).ToList();
            return Ok(list);
        }
        private List<PropertyViewModel> GetActiveProperties()
        {
            var c = _context.ChildPreoperties.ToList();
            c.Insert(0, new ChildPreoperty
            {
                ProID = 0,
                ID = 0,
                Name = "-- Select --"
            });
            var list = (from p in _context.Property.Where(i => i.CompanyID == GetCompany().ID && i.Active)
                        let cps = c.Where(c => c.ProID == p.ID || c.ID == 0).OrderBy(i => i.Name).ToList()
                        select new PropertyViewModel
                        {
                            UnitID = DateTime.Now.Millisecond.ToString() + "" + p.ID,
                            ProID = p.ID,
                            NameProp = p.Name,
                            Values = cps.Count < 0 ? new List<SelectListItem>() : cps.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.Name,
                            }).ToList(),
                            //Value = cp.FirstOrDefault() == null ? 0 : cp.FirstOrDefault().ID
                        }
                        ).ToList();
            return list;
        }
        private List<PropertyViewModel> GetActivePropertiesEdit(int itemID)
        {
            var cp = _context.ChildPreoperties.ToList();
            cp.Insert(0, new ChildPreoperty
            {
                ProID = 0,
                ID = 0,
                Name = "-- Select --"
            });
            var list = (from p in _context.Property.Where(i => i.CompanyID == GetCompany().ID && i.Active)
                        let cpd = _context.PropertyDetails.Where(i => i.ItemID == itemID && i.ProID == p.ID).FirstOrDefault() ?? new PropertyDetails()
                        let cps = cp.Where(c => c.ProID == p.ID || c.ID == 0).OrderBy(i => i.Name).ToList()
                        select new PropertyViewModel
                        {
                            ID = cpd.ID,
                            ProID = p.ID,
                            UnitID = $"{DateTime.Now.Millisecond}{p.ID}{cpd.ItemID}",
                            ItemID = cpd.ItemID,
                            NameProp = p.Name,
                            Values = cps.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.Name,
                                Selected = cpd.Value == c.ID
                            }).ToList(),
                            //Values = new SelectList(cp, "ID", "Name", p.ID),
                            Value = cpd.Value
                        }
                        ).ToList();
            return list;
        }



        [Privilege("A016")]
        public async Task<IActionResult> Create()
        {
            ViewBag.style = "fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            await SetViewBagsAsync();
            return View(new ItemAccountingView
            {
                ItemMasterData = new ItemMasterData
                {
                    Code = _itemMasterData.CreateItemCode()
                },
                ItemAccountings = CreateListItemAccountingsWithNOWarehouse(),
                PropertyDetails = GetActiveProperties()
            });
        }
        List<ItemAccounting> CreateListItemAccountings()
        {
            List<ItemAccounting> itemAccountings = new();
            var warehouses = _context.Warehouses.Where(i => !i.Delete).ToList();
            warehouses.ForEach(w =>
            {
                itemAccountings.Add(new ItemAccounting
                {
                    ID = 0,
                    WarehouseID = w.ID,
                    ItemID = 0,
                    InStock = 0,
                    Ordered = 0,
                    Committed = 0,
                    Available = 0,
                    MinimunInventory = 0,
                    MaximunInventory = 0,
                    Type = "",
                    Code = "",
                    ExpenseAccount = "",
                    RevenueAccount = "",
                    InventoryAccount = "",
                    CostofGoodsSoldAccount = "",
                    AllocationAccount = "",
                    VarianceAccount = "",
                    PriceDifferenceAccount = "",
                    NegativeInventoryAdjustmentAcct = "",
                    InventoryOffsetDecreaseAccount = "",
                    InventoryOffsetIncreaseAccount = "",
                    SalesReturnsAccount = "",
                    RevenueAccountEU = "",
                    ExpenseAccountEU = "",
                    RevenueAccountForeign = "",
                    ExpenseAccountForeign = "",
                    ExchangeRateDifferencesAccount = "",
                    GoodsClearingAccount = "",
                    GLDecreaseAccount = "",
                    GLIncreaseAccount = "",
                    WIPInventoryAccount = "",
                    WIPInventoryVarianceAccount = "",
                    WIPOffsetPLAccount = "",
                    InventoryOffsetPLAccount = "",
                    ExpenseClearingAccount = "",
                    StockInTransitAccount = "",
                    ShippedGoodsAccount = "",
                    SalesCreditAccount = "",
                    PurchaseCreditAccount = "",
                    SalesCreditAccountForeign = "",
                    PurchaseCreditAccountForeign = "",
                    SalesCreditAccountEU = "",
                    PurchaseCreditAccountEU = "",
                    SetGlAccount = 0,
                    Warehouse = warehouses.FirstOrDefault()
                });
            });
            return itemAccountings;
        }

        List<ItemAccounting> CreateListItemAccountingsWithNOWarehouse()
        {
            List<ItemAccounting> itemAccountings = new();
            var warehouses = _context.Warehouses.Where(w => !w.Delete).ToList();
            warehouses.ForEach(w =>
            {
                itemAccountings.Add(new ItemAccounting
                {
                    ID = 0,
                    WarehouseID = w.ID,
                    ItemID = 0,
                    InStock = 0,
                    Ordered = 0,
                    Committed = 0,
                    Available = 0,
                    MinimunInventory = 0,
                    MaximunInventory = 0,
                    Type = "",
                    Code = "",
                    ExpenseAccount = "",
                    RevenueAccount = "",
                    InventoryAccount = "",
                    CostofGoodsSoldAccount = "",
                    AllocationAccount = "",
                    VarianceAccount = "",
                    PriceDifferenceAccount = "",
                    NegativeInventoryAdjustmentAcct = "",
                    InventoryOffsetDecreaseAccount = "",
                    InventoryOffsetIncreaseAccount = "",
                    SalesReturnsAccount = "",
                    RevenueAccountEU = "",
                    ExpenseAccountEU = "",
                    RevenueAccountForeign = "",
                    ExpenseAccountForeign = "",
                    ExchangeRateDifferencesAccount = "",
                    GoodsClearingAccount = "",
                    GLDecreaseAccount = "",
                    GLIncreaseAccount = "",
                    WIPInventoryAccount = "",
                    WIPInventoryVarianceAccount = "",
                    WIPOffsetPLAccount = "",
                    InventoryOffsetPLAccount = "",
                    ExpenseClearingAccount = "",
                    StockInTransitAccount = "",
                    ShippedGoodsAccount = "",
                    SalesCreditAccount = "",
                    PurchaseCreditAccount = "",
                    SalesCreditAccountForeign = "",
                    PurchaseCreditAccountForeign = "",
                    SalesCreditAccountEU = "",
                    PurchaseCreditAccountEU = "",
                    SetGlAccount = 0,
                });
            });
            return itemAccountings;
        }

        private void ValidItemMasterdata(ItemMasterData itemMasterData, List<ItemAccounting> itemAccountings)
        {
            var barcode_old = _context.ItemMasterDatas.AsNoTracking().FirstOrDefault(w => w.ID == itemMasterData.ID);
            if (itemMasterData.PriceListID == 0)
            {
                ModelState.AddModelError("PriceListID", _culLocal["Please select pricelist !"]);
            }

            if (string.IsNullOrWhiteSpace(itemMasterData.Code))
            {
                ModelState.AddModelError("Code", _culLocal["Please Input Code!"]);
            }

            if (_context.ItemMasterDatas.AsNoTracking().Any(i => i.ID != itemMasterData.ID
             && string.Compare(i.Code, itemMasterData.Code, true) == 0))
            {
                ModelState.AddModelError("Code", _culLocal["Code[" + itemMasterData.Code + "] is existing."]);
            }

            if (!string.IsNullOrWhiteSpace(itemMasterData.Barcode))
            {
                if (_context.ItemMasterDatas.AsNoTracking().Any(i => i.ID != itemMasterData.ID
                && string.Compare(i.Barcode, itemMasterData.Barcode, true) == 0))
                {
                    ModelState.AddModelError("Barcode", _culLocal["Barcode[" + itemMasterData.Barcode + "] is existing."]);
                }
            }

            if (itemMasterData.GroupUomID == 0)
            {
                ModelState.AddModelError("GroupUomID", _culLocal["Please select  group uom !"]);
            }

            if (itemMasterData.ItemGroup1ID == 0)
            {
                ModelState.AddModelError("ItemGroup1ID", _culLocal["Please select item group ( 1 ) !"]);
            }

            if (itemMasterData.Type == "0" || itemMasterData.Type == null)
            {
                ModelState.AddModelError("Type", _culLocal["Please select type !"]);
            }

            if (itemMasterData.PrintToID == 0 || itemMasterData.PrintToID == null)
            {
                ModelState.AddModelError("PrintToID", _culLocal["Please select printer name !"]);
            }

            if (itemMasterData.Process == "0" || itemMasterData.Process == null)
            {
                ModelState.AddModelError("PrintToID", _culLocal["Please select process !"]);
            }

            if (itemMasterData.ManageExpire == null)
            {
                ModelState.AddModelError("ManageExpire", _culLocal["Please select manage expire !"]);
            }

            if (itemMasterData.WarehouseID == 0)
            {
                ModelState.AddModelError("WarehouseID", _culLocal["Please select warehouse !"]);
            }

            if (!itemMasterData.Inventory)
            {
                if (itemMasterData.ManItemBy > 0)
                {
                    if (itemMasterData.ManMethod == 0)
                    {
                        ModelState.AddModelError("ManMethod", _culLocal["Management Method cannot be None option!"]);
                    }
                    ModelState.AddModelError("ManItemBy", _culLocal["Non-inventory item cannot be managed by batch or serial numbers!"]);
                }
            }

            if (itemMasterData.SetGlAccount == SetGlAccountMaster.ItemLevel && itemAccountings.Count > 0)
            {
                foreach (var itemAccount in itemAccountings)
                {
                    if (itemAccount.RevenueAccount == null)
                    {
                        ModelState.AddModelError("RevenueAccount ", _culLocal["Please choose RevenueAccount !"]);

                    }
                    if (itemAccount.InventoryAccount == null)
                    {
                        ModelState.AddModelError("InventoryAccount ", _culLocal["Please choose InventoryAccount !"]);

                    }
                    if (itemAccount.CostofGoodsSoldAccount == null)
                    {
                        ModelState.AddModelError("CostofGoodsSoldAccount ", _culLocal["Please choose CostofGoodsSoldAccount !"]);

                    }
                    if (itemAccount.AllocationAccount == null)
                    {
                        ModelState.AddModelError("AllocationAccount ", _culLocal["Please choose AllocationAccount !"]);

                    }
                }
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemMasterData itemMasterData, List<ItemAccounting> itemAccountings, string properties)
        {
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";

            ModelMessage msg = new();

            ValidItemMasterdata(itemMasterData, itemAccountings);
            if (!ModelState.IsValid)
            {
                return Ok(msg.Bind(ModelState));
            }
            else
            {
                await _itemMasterData.CreateAsync(GetUserID(), GetCompanyID(), itemMasterData, itemAccountings, properties, ModelState);
                msg.Approve();
                msg.AddItem(itemMasterData, "ItemMasterData");
                return Ok(msg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImg(IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var file = image;
                    var itemMasterId = int.Parse(Request.Form["ItemMasterId"].ToString());
                    var itemMasterData = _context.ItemMasterDatas.Find(itemMasterId);
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        itemMasterData.Image = fileName;

                        using var fileStream = new FileStream(Path.Combine(GetImagePath(), itemMasterData.Image), FileMode.Create);
                        await file.CopyToAsync(fileStream);
                        _context.SaveChangesAsync().Wait();
                    }
                }
            }
            return Ok();
        }

        [Privilege("A016")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ItemMasterData = "highlight";
            var itemMasterData = _itemMasterData.GetbyId(id);
            var details = _context.ItemAccountings.AsNoTracking().Where(i => i.ItemID == id).Include(e => e.Warehouse).ToList();
            var contract = _context.Contracts.Find(itemMasterData.ContractID) ?? new ContractTemplate();
            var ItemMasterData = _context.ItemMasterDatas.Find(itemMasterData.ID);
            var pldetail = _context.PriceListDetails.FirstOrDefault(i => i.ItemID == itemMasterData.ID && i.UomID == itemMasterData.InventoryUoMID);

            if (pldetail != null)
            {
                pldetail.Barcode = itemMasterData.Barcode;
                _context.PriceListDetails.Update(pldetail);
                _context.SaveChanges();
            }
            if (itemMasterData == null)
            {
                return NotFound();
            }
            var uoms = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == ItemMasterData.GroupUomID)
                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                        select new UOMSViewModel
                        {
                            BaseUoMID = GDU.BaseUOM,
                            Factor = GDU.Factor,
                            ID = UNM.ID,
                            Name = UNM.Name
                        }).ToList();
            ViewBag.Code = itemMasterData.Code;
            ViewBag.Barcode = itemMasterData.Barcode;
            var check = _context.WarehouseDetails.Where(x => x.ItemID == id).Sum(i => i.InStock);
            await SetViewBagsAsync(ItemMasterData);
            if (check > 0 || check < 0)
            {
                ViewBag.Check = "Yes";
                ViewBag.Type = itemMasterData.Type;
                ViewBag.Process = itemMasterData.Process;
                return View(new ItemAccountingView
                {
                    ItemMasterData = itemMasterData,
                    Contract = contract,
                    ItemAccountings = details.Count == 0 ? CreateListItemAccountings() : details,
                    PropertyDetails = GetActivePropertiesEdit(id).Count == 0 ? GetActiveProperties() : GetActivePropertiesEdit(id),
                });
            }
            else
            {
                ViewBag.Process = itemMasterData.Process;
                return View(new ItemAccountingView
                {
                    ItemMasterData = itemMasterData,
                    Contract = contract,
                    ItemAccountings = details.Count == 0 ? CreateListItemAccountings() : details,
                    PropertyDetails = GetActivePropertiesEdit(id).Count == 0 ? GetActiveProperties() : GetActivePropertiesEdit(id),
                });
            }
        }

        [Privilege("A016")]
        [HttpGet]
        public async Task<IActionResult> Copy(int id)
        {
            ViewBag.style = "fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data";
            ViewBag.type = "Copy";
            ViewBag.button = "fa-edit";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            var details = _context.ItemAccountings.AsNoTracking().Where(i => i.ItemID == id).Include(e => e.Warehouse).ToList();
            var itemMasterData = _itemMasterData.GetbyId(id);
            itemMasterData.EnglishName = itemMasterData.EnglishName ?? string.Empty;
            var contract = _context.Contracts.Find(itemMasterData.ContractID) ?? new ContractTemplate();
            var pldetail = _context.PriceListDetails.FirstOrDefault(i => i.ItemID == itemMasterData.ID && i.UomID == itemMasterData.InventoryUoMID);
            if (pldetail != null)
            {
                pldetail.Barcode = itemMasterData.Barcode;
                _context.PriceListDetails.Update(pldetail);
                _context.SaveChanges();
            }
            if (itemMasterData == null)
            {
                return NotFound();
            }
            ViewBag.Code = itemMasterData.Code;
            ViewBag.Barcode = itemMasterData.Barcode;
            var check = _context.InventoryAudits.Where(x => x.ItemID == id).Count();
            await SetViewBagsAsync(itemMasterData);
            if (check > 0)
            {
                ViewBag.Check = "Yes";
                ViewBag.Type = itemMasterData.Type;
                ViewBag.Process = itemMasterData.Process;
                return View(new ItemAccountingView
                {
                    ItemMasterData = itemMasterData,
                    Contract = contract,
                    ItemAccountings = details.Count == 0 ? CreateListItemAccountings() : details,
                    PropertyDetails = GetActivePropertiesEdit(id).Count == 0 ? GetActiveProperties() : GetActivePropertiesEdit(id),
                });
            }
            else
            {
                return View(new ItemAccountingView
                {
                    ItemMasterData = itemMasterData,
                    Contract = contract,
                    ItemAccountings = details.Count == 0 ? CreateListItemAccountings() : details,
                    PropertyDetails = GetActivePropertiesEdit(id).Count == 0 ? GetActiveProperties() : GetActivePropertiesEdit(id),
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string itemMasterData, string itemAccountings, string properties)
        {
            ViewBag.ItemMasterData = "highlight";
            ModelMessage msg = new();
            ItemMasterData __itemMasterData = JsonConvert.DeserializeObject<ItemMasterData>(itemMasterData,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<ItemAccounting> __itemAccountings = JsonConvert.DeserializeObject<List<ItemAccounting>>(itemAccountings,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<PropertyDetails> _properties = JsonConvert.DeserializeObject<List<PropertyDetails>>(properties,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            var group1 = _context.ItemGroup1.Find(__itemMasterData.ItemGroup1ID).ItemG1ID;
            var itemAcc = _context.ItemAccountings.AsNoTracking().Where(i => i.ItemGroupID == group1).FirstOrDefault();

            if (__itemMasterData.SetGlAccount == SetGlAccountMaster.ItemGroup)
            {
                if (itemAcc == null)
                {
                    ModelState.AddModelError("GroupUomID", "Item Group has to have Accounting with!!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            ValidItemMasterdata(__itemMasterData, __itemAccountings);
            if (!ModelState.IsValid)
            {
                return Ok(msg.Bind(ModelState));
            }
            else
            {
                using var t = _context.Database.BeginTransaction();
                var CheckItemProcess = _context.InventoryAudits.AsNoTracking().Where(x => x.ItemID == __itemMasterData.ID).Count();
                var itemMaster = _context.ItemMasterDatas.AsNoTracking().FirstOrDefault(w => w.ID == __itemMasterData.ID);
                // var defiendUom = _context.GroupDUoMs.AsNoTracking().Where(x => x.GroupUoMID == __itemMasterData.GroupUomID).ToList();
                var defiendUom = _context.GroupDUoMs.AsNoTracking().Where(x => x.GroupUoMID == __itemMasterData.GroupUomID && x.Factor == 1).ToList();
                if (__itemMasterData.GroupUomID != itemMaster.GroupUomID || __itemMasterData.Process != itemMaster.Process)
                {
                    _itemMasterData.RemoveItmeInWarehous(__itemMasterData.ID);
                    var currency = _context.PriceLists.AsNoTracking().FirstOrDefault(x => x.ID == __itemMasterData.PriceListID && x.Delete == false);
                    var companycur = (from com in _context.Company.Where(x => !x.Delete)
                                      join cur in _context.Currency.Where(x => !x.Delete) on com.SystemCurrencyID equals cur.ID
                                      select new
                                      {
                                          CurrencyID = cur.ID
                                      }).ToList();

                    var SysCurrency = 0;
                    foreach (var item in companycur)
                    {
                        SysCurrency = item.CurrencyID;
                    }
                    //Warenouse Summary
                    if (__itemMasterData.Process != "Standard")
                    {
                        WarehouseSummary warehouseSummary = new()
                        {
                            WarehouseID = __itemMasterData.WarehouseID,
                            ItemID = __itemMasterData.ID,
                            InStock = 0,
                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                            SyetemDate = Convert.ToDateTime(DateTime.Today),
                            UserID = int.Parse(User.FindFirst("UserID").Value),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                            CurrencyID = companycur.FirstOrDefault().CurrencyID,
                            UomID = Convert.ToInt32(__itemMasterData.InventoryUoMID),
                            Cost = 0,
                            Available = 0,
                            Committed = 0,
                            Ordered = 0
                        };
                        await _itemMasterData.AddWarehouseSummary(warehouseSummary);
                    }

                    foreach (var item in defiendUom)
                    {
                        //Standard
                        if (__itemMasterData.Process == "Standard")
                        {
                            if (item.AltUOM == __itemMasterData.SaleUomID)
                            {
                                PriceListDetail priceListDetail = new()
                                {
                                    ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                                    SystemDate = Convert.ToDateTime(DateTime.Today),
                                    TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                                    UserID = int.Parse(User.FindFirst("UserID").Value),
                                    UomID = __itemMasterData.SaleUomID,
                                    CurrencyID = currency.CurrencyID,
                                    ItemID = __itemMasterData.ID,
                                    PriceListID = __itemMasterData.PriceListID,
                                    Cost = __itemMasterData.Cost,
                                    UnitPrice = __itemMasterData.UnitPrice,
                                    Barcode = __itemMasterData.Barcode
                                };
                                await _itemMasterData.AddPricelistDetail(priceListDetail);
                            }
                            else
                            {
                                PriceListDetail priceListDetail = new()
                                {
                                    ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                                    SystemDate = Convert.ToDateTime(DateTime.Today),
                                    TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                                    UserID = int.Parse(User.FindFirst("UserID").Value),
                                    UomID = item.AltUOM,
                                    CurrencyID = currency.CurrencyID,
                                    ItemID = __itemMasterData.ID,
                                    PriceListID = __itemMasterData.PriceListID,
                                    Cost = 0,
                                    UnitPrice = 0,
                                    Barcode = __itemMasterData.Barcode
                                };
                                await _itemMasterData.AddPricelistDetail(priceListDetail);
                            }
                        }
                        //FIFO, Average, FEFO, SEBA (Serial/Batch)
                        else
                        {
                            if (item.AltUOM == __itemMasterData.SaleUomID)
                            {
                                PriceListDetail priceListDetail = new()
                                {
                                    ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                                    SystemDate = Convert.ToDateTime(DateTime.Today),
                                    TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                                    UserID = int.Parse(User.FindFirst("UserID").Value),
                                    UomID = __itemMasterData.SaleUomID,
                                    CurrencyID = currency.CurrencyID,
                                    ItemID = __itemMasterData.ID,
                                    PriceListID = __itemMasterData.PriceListID,
                                    Cost = __itemMasterData.Cost,
                                    UnitPrice = __itemMasterData.UnitPrice,
                                    Barcode = __itemMasterData.Barcode
                                };
                                await _itemMasterData.AddPricelistDetail(priceListDetail);
                            }
                            else
                            {
                                PriceListDetail priceListDetail = new()
                                {
                                    ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                                    SystemDate = Convert.ToDateTime(DateTime.Today),
                                    TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                                    UserID = int.Parse(User.FindFirst("UserID").Value),
                                    UomID = item.AltUOM,
                                    CurrencyID = currency.CurrencyID,
                                    ItemID = __itemMasterData.ID,
                                    PriceListID = __itemMasterData.PriceListID,
                                    Cost = 0,
                                    UnitPrice = 0,
                                    Barcode = __itemMasterData.Barcode
                                };
                                await _itemMasterData.AddPricelistDetail(priceListDetail);
                            }
                            //Insert to warehoues detail
                            WarehouseDetail warehouseDetail = new()
                            {
                                WarehouseID = __itemMasterData.WarehouseID,
                                ItemID = __itemMasterData.ID,
                                InStock = 0,
                                ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                                SyetemDate = Convert.ToDateTime(DateTime.Today),
                                UserID = int.Parse(User.FindFirst("UserID").Value),
                                TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                                CurrencyID = companycur.FirstOrDefault().CurrencyID,
                                UomID = item.AltUOM,
                                Cost = 0,
                            };
                            await _itemMasterData.AddWarehouseDeatail(warehouseDetail);
                        }
                    }
                }
                else
                {
                    foreach (var gud in defiendUom)
                    {
                        var pld = _context.PriceListDetails
                          .FirstOrDefault(pd => pd.ItemID == __itemMasterData.ID && pd.UomID == gud.UoMID);
                        if (pld != null)
                        {
                            pld.Barcode = __itemMasterData.Barcode;
                            _context.PriceListDetails.Update(pld);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                __itemMasterData.UnitofMeasureSale = null;
                __itemMasterData.UnitofMeasurePur = null;
                __itemMasterData.UnitofMeasureInv = null;
                await _itemMasterData.AddOrEdit(__itemMasterData, ModelState);


                foreach (var item in __itemAccountings)
                {
                    if (__itemMasterData.ID == item.ItemID)
                    {
                        item.Warehouse = null;
                        _context.ItemAccountings.Update(item);
                    }
                    else
                    {
                        item.Warehouse = null;
                        item.ItemID = __itemMasterData.ID;
                        _context.ItemAccountings.Add(item);
                    }
                    _context.SaveChanges();
                }
                foreach (var item in _properties)
                {
                    if (__itemMasterData.ID == item.ItemID)
                    {
                        _context.PropertyDetails.Update(item);
                    }
                    else
                    {
                        item.ItemID = __itemMasterData.ID;
                        _context.PropertyDetails.Add(item);
                    };
                    _context.SaveChanges();
                }
                t.Commit();
                msg.AddItem(__itemMasterData, "ItemMasterData");
                msg.Approve();
                return Ok(msg);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Copy(string itemMaster, string itemAccounting, string properties)
        {
            ViewBag.style = "fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            ModelMessage msg = new();
            ItemMasterData __itemMasterData = JsonConvert.DeserializeObject<ItemMasterData>(itemMaster,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<ItemAccounting> __itemAccountings = JsonConvert.DeserializeObject<List<ItemAccounting>>(itemAccounting,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<PropertyDetails> _properties = JsonConvert.DeserializeObject<List<PropertyDetails>>(properties,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            var group1 = _context.ItemGroup1.Find(__itemMasterData.ItemGroup1ID).ItemG1ID;
            var itemAcc = _context.ItemAccountings.AsNoTracking().Where(i => i.ItemGroupID == group1).FirstOrDefault();
            if (__itemMasterData.SetGlAccount == SetGlAccountMaster.ItemGroup)
            {
                if (itemAcc == null)
                {
                    ModelState.AddModelError("ItemGroup1ID", "Item Group has to have Accounting with!!");
                    return Ok(msg.Bind(ModelState));
                }
            }

            ValidItemMasterdata(__itemMasterData, __itemAccountings);
            if (!ModelState.IsValid)
            {
                return Ok(msg.Bind(ModelState));
            }
            else
            {
                using var t = _context.Database.BeginTransaction();
                await _itemMasterData.AddOrEdit(__itemMasterData, ModelState);
                var currency = _context.PriceLists.FirstOrDefault(x => x.ID == __itemMasterData.PriceListID && x.Delete == false);
                var companycur = (from com in _context.Company.Where(x => !x.Delete)
                                  join cur in _context.Currency.Where(x => !x.Delete) on com.SystemCurrencyID equals cur.ID
                                  select new
                                  {
                                      CurrencyID = cur.ID
                                  }).ToList();
                var defiendUom = _context.GroupDUoMs.Where(x => x.GroupUoMID == __itemMasterData.GroupUomID && x.Delete == false).ToList();
                var SysCurrency = 0;
                foreach (var item in companycur)
                {
                    SysCurrency = item.CurrencyID;
                }

                //Warenouse Summary
                if (__itemMasterData.Process != "Standard")
                {
                    WarehouseSummary warehouseSummary = new()
                    {
                        WarehouseID = __itemMasterData.WarehouseID,
                        ItemID = __itemMasterData.ID,
                        InStock = 0,
                        ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                        SyetemDate = Convert.ToDateTime(DateTime.Today),
                        UserID = int.Parse(User.FindFirst("UserID").Value),
                        TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                        CurrencyID = companycur.FirstOrDefault().CurrencyID,
                        UomID = Convert.ToInt32(__itemMasterData.InventoryUoMID),
                        Cost = 0,
                        Available = 0,
                        Committed = 0,
                        Ordered = 0

                    };
                    await _itemMasterData.AddWarehouseSummary(warehouseSummary);
                }

                foreach (var item in defiendUom)
                {
                    //Standard
                    if (__itemMasterData.Process == "Standard")
                    {
                        if (item.AltUOM == __itemMasterData.SaleUomID)
                        {
                            PriceListDetail priceListDetail = new()
                            {

                                ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                                SystemDate = Convert.ToDateTime(DateTime.Today),
                                TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                                UserID = int.Parse(User.FindFirst("UserID").Value),
                                UomID = __itemMasterData.SaleUomID,
                                CurrencyID = currency.CurrencyID,
                                ItemID = __itemMasterData.ID,
                                PriceListID = __itemMasterData.PriceListID,
                                Cost = __itemMasterData.Cost,
                                UnitPrice = __itemMasterData.UnitPrice,
                                Barcode = __itemMasterData.Barcode
                            };
                            await _itemMasterData.AddPricelistDetail(priceListDetail);
                        }
                        else
                        {
                            PriceListDetail priceListDetail = new()
                            {

                                ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")),
                                SystemDate = Convert.ToDateTime(DateTime.Today),
                                TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                                UserID = int.Parse(User.FindFirst("UserID").Value),
                                UomID = item.AltUOM,
                                CurrencyID = currency.CurrencyID,
                                ItemID = __itemMasterData.ID,
                                PriceListID = __itemMasterData.PriceListID,
                                Cost = 0,
                                UnitPrice = 0,
                                Barcode = __itemMasterData.Barcode
                            };
                            await _itemMasterData.AddPricelistDetail(priceListDetail);
                        }

                    }
                    //FIFO, Average, FEFO, SEBA(Serial/Batch)
                    else
                    {

                        if (item.AltUOM == __itemMasterData.SaleUomID)
                        {
                            PriceListDetail priceListDetail = new()
                            {

                                ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                                SystemDate = Convert.ToDateTime(DateTime.Today),
                                TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                                UserID = int.Parse(User.FindFirst("UserID").Value),
                                UomID = __itemMasterData.SaleUomID,
                                CurrencyID = currency.CurrencyID,
                                ItemID = __itemMasterData.ID,
                                PriceListID = __itemMasterData.PriceListID,
                                Cost = __itemMasterData.Cost,
                                UnitPrice = __itemMasterData.UnitPrice,
                                Barcode = __itemMasterData.Barcode
                            };
                            await _itemMasterData.AddPricelistDetail(priceListDetail);

                        }
                        else
                        {
                            PriceListDetail priceListDetail = new()
                            {

                                ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                                SystemDate = Convert.ToDateTime(DateTime.Today),
                                TimeIn = Convert.ToDateTime(DateTime.Now.ToString("h:mm:ss")),
                                UserID = int.Parse(User.FindFirst("UserID").Value),
                                UomID = item.AltUOM,
                                CurrencyID = currency.CurrencyID,
                                ItemID = __itemMasterData.ID,
                                PriceListID = __itemMasterData.PriceListID,
                                Cost = 0,
                                UnitPrice = 0,
                                Barcode = __itemMasterData.Barcode
                            };
                            await _itemMasterData.AddPricelistDetail(priceListDetail);
                        }
                        //Insert to warehoues detail

                        WarehouseDetail warehouseDetail = new()
                        {
                            WarehouseID = __itemMasterData.WarehouseID,
                            ItemID = __itemMasterData.ID,
                            InStock = 0,
                            ExpireDate = Convert.ToDateTime(DateTime.Now.ToString("2019/09/09")),
                            SyetemDate = Convert.ToDateTime(DateTime.Today),
                            UserID = int.Parse(User.FindFirst("UserID").Value),
                            TimeIn = Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss tt")),
                            CurrencyID = companycur.FirstOrDefault().CurrencyID,
                            UomID = item.AltUOM,
                            Cost = 0,
                        };
                        await _itemMasterData.AddWarehouseDeatail(warehouseDetail);
                    }

                }
                ViewData["GroupUomID"] = new SelectList(_context.GroupUOMs.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.GroupUomID);
                ViewData["ItemGroup1ID"] = new SelectList(_context.ItemGroup1.Where(d => d.Delete == false), "ItemG1ID", "Name", __itemMasterData.ItemGroup1ID);
                ViewData["ItemGroup2ID"] = new SelectList(_context.ItemGroup2.Where(d => d.Delete == false && d.Name != "None"), "ItemG2ID", "Name", __itemMasterData.ItemGroup2ID);
                ViewData["ItemGroup3ID"] = new SelectList(_context.ItemGroup3.Where(d => d.Delete == false && d.Name != "None"), "ID", "Name", __itemMasterData.ItemGroup3ID);
                ViewData["PriceListID"] = new SelectList(_context.PriceLists.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.PriceListID);
                ViewData["InventoryUoMID"] = new SelectList(_context.UnitofMeasures.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.InventoryUoMID);
                ViewData["SaleUomID"] = new SelectList(_context.UnitofMeasures.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.SaleUomID);
                ViewData["PurchaseUomID"] = new SelectList(_context.UnitofMeasures.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.PurchaseUomID);
                ViewData["PrintToID"] = new SelectList(_context.PrinterNames.Where(d => d.Delete == false), "ID", "Name", __itemMasterData.PrintToID);
                ViewData["WarehouseID"] = new SelectList(_context.Warehouses.Where(x => x.Delete == false && x.BranchID == Convert.ToInt32(User.FindFirst("BranchID").Value)), "ID", "Name");
                var masterId = __itemMasterData.ID;
                foreach (var accounting in __itemAccountings)
                {
                    if (ModelState.IsValid)
                    {
                        accounting.ItemID = masterId;
                        accounting.Warehouse = null;
                        accounting.ItemMasterData = null;
                        _context.ItemAccountings.Update(accounting);
                        _context.SaveChanges();
                    }
                }
                foreach (var item in _properties)
                {
                    item.ItemID = masterId;
                    _context.PropertyDetails.Update(item);
                    _context.SaveChanges();
                }
                t.Commit();
                msg.Approve();
                msg.AddItem(__itemMasterData, "ItemMasterData");
                return Ok(msg);
            }
        }

        [HttpGet]
        public IActionResult GetPrinter()
        {
            var list = _itemMasterData.GetPrinter.Where(p => p.Delete == false).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult FindeCode(string code)
        {
            var codes = _context.ItemMasterDatas.Where(c => c.Code == code && c.Delete == false);
            return Ok(codes);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteItemMaster(int ID)
        {
            await _itemMasterData.DeleteItemMaster(ID);
            return Ok();
        }
        [HttpGet]
        public IActionResult FindbarCode(string barcode)
        {
            var bar = _context.ItemMasterDatas.Where(c => c.Barcode == barcode && c.Delete == false);
            return Ok(bar);
        }
        [HttpPost]
        public IActionResult DeletItemViewGrid(int ID)
        {
            _itemMasterData.DeleteItemMaster(ID);
            var list = _itemMasterData.GetMasterDatas();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult CheckTransactionItem(int ItemID)
        {
            var check = _context.InventoryAudits.Where(x => x.ItemID == ItemID).Count();
            if (check > 0)
            {
                return Ok("Y");
            }
            else
            {
                return Ok("N");
            }
        }
        //Import/Export
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            ViewBag.style = "fa fa-users";
            ViewBag.Main = "Business Partners";
            ViewBag.Page = "Motocycle Import";
            ViewBag.BizPartnersMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            ModelMessage message = new(ModelState);
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    int userId = GetUserID();
                    int companyId = GetCompanyID();
                    IFormFile file = Request.Form.Files[0];
                    IWorkbook wb = null;
                    ISheet sheet = null;
                    int sheetIndex = int.Parse(Request.Form["SheetIndex"]);
                    Stream fs = new MemoryStream();
                    file.CopyTo(fs);
                    fs.Position = 0;
                    wb = _workbook.ReadWorkbook(fs);
                    sheet = wb.GetSheetAt(sheetIndex);

                    IList<ItemMasterData> ItemMasterData = new List<ItemMasterData>();
                    var jsons = _workbook.Serialize(sheet);
                    ItemMasterData = JsonConvert.DeserializeObject<IList<ItemMasterData>>(jsons);
                    foreach (var item in ItemMasterData)
                    {
                        ValidItemMasterdata(item, new List<ItemAccounting>());
                        if (ModelState.IsValid)
                        {
                            await _itemMasterData.CreateAsync(userId, companyId, item,
                            CreateListItemAccountingsWithNOWarehouse(), "", ModelState);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        message.Add("__success", "Items have been uploaded.");
                        message.Approve();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Exeption", ex.Message);
            }

            return Ok(message.Bind(ModelState));
        }

        public IActionResult UpdateRow(ItemMasterDataUpdateRow itemMaster)
        {
            ModelMessage msg = new();
            var itemMasterData = _context.ItemMasterDatas.Find(itemMaster.ID);
            var pldetail = _context.PriceListDetails.FirstOrDefault(i => i.ItemID == itemMasterData.ID && i.UomID == itemMasterData.InventoryUoMID);
            if (pldetail != null)
            {
                pldetail.Barcode = itemMaster.Barcode;
                _context.PriceListDetails.Update(pldetail);
                _context.SaveChanges();
            }
            if (itemMasterData.Barcode != itemMaster.Barcode)
            {
                var checkBarcode = _context.ItemMasterDatas.FirstOrDefault(w => w.Barcode == itemMaster.Barcode);
                if (checkBarcode != null)
                {
                    //ViewBag.barcodeerror = "This barcode have exist !";
                    msg.Add("barcode", "This barcode already existed.");
                }

            }

            if (msg.Data.Count == 0)
            {
                itemMasterData.Code = itemMaster.Code;
                itemMasterData.KhmerName = itemMaster.KhmerName;
                itemMasterData.EnglishName = itemMaster.EnglishName;
                itemMasterData.Barcode = itemMaster.Barcode;

                _context.Update(itemMasterData);
                _context.SaveChanges();
                msg.Approve();
            }

            return Ok(msg);
        }

        [HttpPost]
        public IActionResult UploadImage()
        {
            int itemID = int.Parse(Request.Form["ItemID"]);
            IFormFile imageFile = HttpContext.Request.Form.Files[0];
            string image = imageFile.FileName;
            if (imageFile != null && imageFile.Length > 0)
            {
                var itemMaster = _context.ItemMasterDatas.Find(itemID);
                if (itemMaster != null)
                {
                    var file = imageFile;
                    if (file.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(GetImagePath(), image), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        itemMaster.Image = image;
                        _context.SaveChanges();

                    }
                }

            }
            return Ok();
        }

        //Import Item Master Data
        public IActionResult Import()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Item Master Data ";
            ViewBag.Subpage = "Import";
            ViewBag.InventoryMenu = "show";
            ViewBag.ItemMasterData = "highlight";
            return View();
        }

        public IActionResult PreviewComponent()
        {
            var group_defined_uom = from duom in _context.GroupDUoMs.OrderBy(g => g.GroupUoMID)
                                    join uom in _context.UnitofMeasures on duom.AltUOM equals uom.ID
                                    join guom in _context.GroupUOMs.Where(w => w.Delete == false) on duom.GroupUoMID equals guom.ID
                                    select new
                                    {
                                        duom.ID,
                                        duom.GroupUoMID,
                                        duom.AltUOM,
                                        uom.Name,
                                        duom.Factor
                                    };
            var itemMaster = new
            {
                PriceLists = _context.PriceLists.Where(w => w.Delete == false),
                GroupUOMs = _context.GroupUOMs.Where(w => w.Delete == false),
                GroupDefinedUoM = group_defined_uom,
                PrinterNames = _context.PrinterNames.Where(w => w.Delete == false),
                Warehouses = _context.Warehouses.Where(w => w.Delete == false),
                ItemGroup1 = _context.ItemGroup1.Where(ig1 => ig1.Delete == false && string.Compare(ig1.Name, "None", true) != 0).OrderBy(g1 => g1.ItemG1ID),
                ItemGroup2 = _context.ItemGroup2.Where(ig2 => ig2.Delete == false && string.Compare(ig2.Name, "None", true) != 0).OrderBy(g2 => g2.ItemG1ID),
                ItemGroup3 = _context.ItemGroup3.Where(ig3 => ig3.Delete == false && string.Compare(ig3.Name, "None", true) != 0).OrderBy(g3 => g3.ItemG1ID).OrderBy(g3 => g3.ItemG2ID)
            };
            return Ok(itemMaster);
        }


        [HttpPost]
        public IActionResult GetFileNames()
        {
            ModelMessage message = new(ModelState);
            IList<string> sheetNames = new List<string>();
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files[0];
                if (ModelState.IsValid)
                {
                    using Stream fs = new MemoryStream();
                    file.CopyTo(fs);
                    fs.Position = 0;
                    IWorkbook wb = WorkbookFactory.Create(fs);
                    for (int i = 0; i < wb.NumberOfSheets; i++)
                    {
                        sheetNames.Add(wb.GetSheetAt(i).SheetName);
                    }
                }
            }

            return Ok(new { SheetNames = sheetNames, Message = message });
        }

        public async Task<IActionResult> Download()
        {
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _appEnvironment.WebRootPath, "/FileTemplate/template.xlsx"));

            var memory = new MemoryStream();
            using Stream fs = System.IO.File.OpenRead(fullPath);
            await fs.CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(fullPath));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportItemMasters(FileDirectory directory)
        {
            ModelMessage message = new(ModelState);
            RNGCryptoServiceProvider rng = new();
            byte[] randomBytes = new byte[12];
            rng.GetBytes(randomBytes);
            string ext = ".xlsx, .xls";
            string fileName = directory.FileName ?? Convert.ToBase64String(randomBytes);
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Contains(ext.Split(".")[1]) ? fileName : fileName + directory.FileType;
            }

            _ = string.Format(@"{0}\{1}", _appEnvironment.WebRootPath, fileName);
            return Ok(message.Bind(ModelState));
        }

        private void ValidationContractTemplate(ContractTemplate contract)
        {
            if (string.IsNullOrEmpty(contract.Name))
            {
                ModelState.AddModelError("Name", "Name is required!");
            }
            //if (contract.ContracType == ContractType.None)
            //{
            //    ModelState.AddModelError("ContractType", "Contract Type is required!");
            //}
            //if (contract.Duration < 0)
            //{
            //    ModelState.AddModelError("Duration", "Duration must be greater than 0 or not empty!");
            //}
        }
        public IActionResult GetContractType()
        {
            var list = _context.SetupContractTypes.ToList();
            return Ok(list);
        }
        public async Task<IActionResult> CreateContractTemplate(ContractTemplate contract)
        {
            ModelMessage msg = new();
            ValidationContractTemplate(contract);
            if (ModelState.IsValid)
            {
                await _itemMasterData.CreateContractTemplate(contract);
                msg.Approve();
                ModelState.AddModelError("success", "Contract template created succussfully!");
            }
            var data = (from con in _context.Contracts
                        let cont = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType)
                        select new
                        {
                            ID = con.ID,
                            Name = con.Name,
                            ContractType = cont.ContractType /*== ContractType.Customer.ToString() ? "Customer" : cont.ContractType == ContractType.ItemGroup.ToString() ?"Item Group" : cont.ContractType == ContractType.SerialNumber.ToString() ? "Serial Number" : ""*/
                        }).ToList();
            msg.AddItem(data, "Contract");
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult GetAllContractTemplates()
        {
            var data = (from con in _context.Contracts
                        let cont = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType)
                        select new
                        {
                            ID = con.ID,
                            Name = con.Name,
                            ContractType = cont.ContractType /*== ContractType.Customer.ToString() ? "Customer" : cont.ContractType == ContractType.ItemGroup.ToString() ?"Item Group" : cont.ContractType == ContractType.SerialNumber.ToString() ? "Serial Number" : ""*/
                        }).ToList();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult PreviewItemMasterData()
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files[0];
                using Stream fs = new MemoryStream();
                int sheetIndex = int.Parse(Request.Form["SheetIndex"]);
                file.CopyTo(fs);
                fs.Position = 0;
                IWorkbook wb = _workbook.ReadWorkbook(fs);
                return Ok(_workbook.ParseKeyValue(wb.GetSheetAt(sheetIndex)));
            }
            return Ok(new List<Dictionary<string, string>>());

        }

        public async Task<IActionResult> Export()
        {
            var getItemMaster = await (from item in _context.ItemMasterDatas.Where(d => !d.Delete)
                                        join u in _context.UnitofMeasures.Where(d => !d.Delete) on item.InventoryUoMID equals u.ID
                                        join ig in _context.ItemGroup1.Where(d => !d.Delete) on item.ItemGroup1ID equals ig.ItemG1ID
                                        select new ItemMasterDataExport{
                                            Code = item.Code,
                                            ItemName1 = item.KhmerName,
                                            ItemName2 = item.EnglishName,
                                            UoM = u.Name,
                                            ItemGroup1 = ig.Name,
                                            Barcode = item.Barcode,
                                            Type = item.Process,
                                            Stock = item.StockIn
                                        }).ToListAsync();
            _wbExport.AddSheet(getItemMaster);
            Stream ms = new MemoryStream();
            _wbExport.Write(ms);
            ms.Position=0;
            return File(ms,"application/octet-stream","ItemMasterDataExport.xls");
        }

    }
}