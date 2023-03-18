
using System;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Inventory.Transaction;
using KEDI.Core.Premise.Models.Services.Repositories;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KEDI.Core.Premise.Controllers
{
    public class InventoryCountingController : Controller
    {

        private readonly DataContext _context;
        private readonly IinventoryCounting _invCounting;
        private readonly UtilityModule _utility;

        public InventoryCountingController(DataContext context, UtilityModule utility, IinventoryCounting invCounting)
        {

            _context = context;
            _invCounting = invCounting;
            _utility = utility;
        }
        public async Task<IActionResult> AddRowINVCounting()
        {
            var obj = _invCounting.CreateDefualtRow();
            return Ok(await Task.FromResult(obj));
        }
        public async Task<IActionResult> BindRows()
        {
            var list = await _invCounting.BindRows();
            return Ok(list);
        }


        public IActionResult InventoryCounting()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main  = "Inventory";
            ViewBag.Page  = "Transaction";
            ViewBag.Subpage = "InventoryCounting";
            ViewBag.InventoryMenu = "show";
            ViewBag.Transaction = "show";
            ViewBag.InventoryCounting = "highlight";

            return View(new { seriesIC = _utility.GetSeries("IC"), seriesJE = _utility.GetSeries("JE"), GeneralSetting = GetGeneralSettingAdmin().Display });
        }
        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;

        }
        public async Task<IActionResult> GetBranch()
        {
            var branch = await _context.Branches.Where(s => !s.Delete).ToListAsync();
            return Ok(branch);
        }
        public async Task<IActionResult> GetItemMaster(string barcode = "")
        {
            var list = await _invCounting.GetItemMaster(barcode);
            return Ok(list);
        }
        public IActionResult GetStockFromWarehouse(int itemID, int wID, int uomID)
        {
            var obj = _invCounting.GetStockFromWarehouse(itemID, wID, uomID);
            return Ok(obj);
        }
        public async Task<IActionResult> GetEmployee()
        {
            var listem = await _invCounting.GetEmployee();
            return Ok(listem);
        }
        public async Task<IActionResult> SaveINVCounting(InventoryCounting invcounting, string je)
        {
            ModelMessage msg = new();
            //  InventoryCounting invcounting = JsonConvert.DeserializeObject<InventoryCounting>(objstr, new JsonSerializerSettings
            // {
            //     NullValueHandling = NullValueHandling.Ignore
            // });
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (invcounting.Date.Year == 1)
                ModelState.AddModelError("Date", "please choose Count Date ...!");
            if (invcounting.Time.Hours == 0)
                ModelState.AddModelError("Time", "please input Time ...!");
            if (invcounting.BranchID == 0)
                ModelState.AddModelError("BranchID", "please select Branch  ...!");
            if (invcounting.DocTypeID == 0)
                ModelState.AddModelError("DocTypeID", "please select Number  ...!");
            if (string.IsNullOrWhiteSpace(invcounting.InvioceNumber))
                ModelState.AddModelError("InvioceNumber", "please input Number  ...!");
            if (string.IsNullOrWhiteSpace(invcounting.Status))
                ModelState.AddModelError("Status", "please input Status  ...!");
            invcounting.InventoryCountingDetails = invcounting.InventoryCountingDetails.Where(s => s.ItemID > 0).ToList();
            if (invcounting.InventoryCountingDetails.Count == 0)
                ModelState.AddModelError("", "please input Inventory counting Detail  ...!");
            else
                foreach (var invd in invcounting.InventoryCountingDetails)
                {
                    if (invd.ItemID == 0)
                        ModelState.AddModelError("ItemID", "please input Item in Inventory Counting Detail  ...!");
                    if (invd.WarehouseID == 0)
                        ModelState.AddModelError("WarehouseID", "please select Warehouse in Inventory Counting Detail  ...!");
                    if (invd.UomCountQty == 0)
                        ModelState.AddModelError("UomCountQty", "please input Uom Count Qty  in Inventory Counting Detail  ...!");
                    if (invd.EmployeeID == 0)
                        ModelState.AddModelError("EmployeeID", "please input Counted By in Inventory Counting Detail  ...!");
                }
            SeriesDetail seriesDetail = new();
            var seriesPS = _context.Series.FirstOrDefault(w => w.ID == invcounting.SeriesID);
            var douType  = _context.DocumentTypes.Where(i => i.ID == invcounting.DocTypeID).FirstOrDefault();
            var seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                if (invcounting.ID == 0)
                {
                    seriesDetail.Number = seriesPS.NextNo;
                    seriesDetail.SeriesID = invcounting.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();


                    var seriesDetailID = seriesDetail.ID;
                    invcounting.SeriesDetailID = seriesDetail.ID;
                    string Sno = seriesDetail.Number;
                    long No = long.Parse(Sno);
                    seriesPS.NextNo = Convert.ToString(No + 1);
                    if (No > long.Parse(seriesPS.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Inventory Counting Invoice has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }
                    // checking maximun Invoice
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                }
                _context.Update(invcounting);
                await _context.SaveChangesAsync();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", "Data Saved successfully !!");
                t.Commit();
            }

            return Ok(new { Model = msg.Bind(ModelState) });
        }

        public async Task<IActionResult> FindInventoryCounting(string number)
        {
            var obj = await _invCounting.FindInventoryCounting(number);
            if (obj != null)
                return Ok(obj);
            return Ok();

        }
    }
}