using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Newtonsoft.Json;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Repository;
using KEDI.Core.System.Models;

namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class GoodIssuesController : Controller
    {
        private readonly DataContext _context;
        private readonly IGoodIssuse _issuse;
        private readonly ISaleSerialBatchRepository _saleSerialBatch;
        private readonly UtilityModule _utilityModule;

        public GoodIssuesController(DataContext context, IGoodIssuse issuse, ISaleSerialBatchRepository saleSerialBatch, UtilityModule utilityModule)
        {
            _context = context;
            _issuse = issuse;
            _saleSerialBatch = saleSerialBatch;
            _utilityModule = utilityModule;
        }
        [Privilege("A040")]
        public IActionResult GoodIssues()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Transaction";
            ViewBag.Subpage = "Goods Issue";
            ViewBag.InventoryMenu = "show";
            ViewBag.Transaction = "show";
            ViewBag.GoodsIssue = "highlight";
            return View(new { seriesGI = _utilityModule.GetSeries("GI"), seriesJE = _utilityModule.GetSeries("JE") });
        }
        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }
        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        [HttpGet]
        public IActionResult GetWarehousesFrom(int ID)
        {
            var list = _issuse.GetWarehouse_From(ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetAllPurItemsByItemID(int wareId, int ID)
        {
            var list = _issuse.GetAllPurItemsByItemID(wareId, ID, GetCompany().ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemByWarehouseFrom(int ID)
        {
            var list = _issuse.GetItemByWarehouse_From(ID, GetCompany().ID);
            return Ok(list);
        }
        public IActionResult SaveGoodIssues(GoodIssues issues, string serial, string batch)
        {
            ModelMessage msg = new();
            var lc = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID) ?? new ExchangeRate();
            SeriesDetail seriesDetail = new();
            // insert Series Detail
            var DouType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "GI");
            var Series = _context.Series.Find(issues.SeriseID);
            seriesDetail.Number = Series.NextNo;
            seriesDetail.SeriesID = Series.ID;
            issues.CompanyID = GetCompany().ID;
            issues.SysCurID = GetCompany().SystemCurrencyID;
            issues.LocalCurID = GetCompany().LocalCurrencyID;
            issues.LocalSetRate = lc.SetRate;
            issues.SeriseID = Series.ID;
            issues.Number_No = seriesDetail.Number;
            issues.DocTypeID = DouType.ID;
               if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                 {
                     ValidatErrorBasic(issues);
                 }
                 else{
                        ValidatError(issues);
                 }
        
            if (ModelState.IsValid)
            {
                // Checking Serial Batch //
                List<SerialNumber> serialNumber = new();
                List<SerialNumber> _serialNumber = new();
                List<BatchNo> batchNoes = new();
                List<BatchNo> _batchNoes = new();
                serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialNumber;

                _saleSerialBatch.CheckItemSerailGoodsIssue(issues, issues.GoodIssuesDetails, serialNumber);
                serialNumber = serialNumber
                    .GroupBy(i => new { i.ItemID, i.Cost })
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialNumber.ToList())
                {
                    foreach (var i in issues.GoodIssuesDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)i.Cost)
                        {
                            _serialNumber.Add(j);
                        }
                    }
                }
                bool isHasSerialItem = _serialNumber.Any(i => i.OpenQty != 0);
                if (isHasSerialItem)
                {
                    return Ok(new { IsSerail = true, Data = _serialNumber });
                }
                batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchNoes;
                _saleSerialBatch.CheckItemBatchGoodsIssue(issues, issues.GoodIssuesDetails, batchNoes);
                batchNoes = batchNoes
                    .GroupBy(i => new { i.ItemID, i.Cost })
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchNoes.ToList())
                {
                    foreach (var i in issues.GoodIssuesDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)i.Cost)
                        {
                            _batchNoes.Add(j);
                        }
                    }
                }
                bool isHasBatchItem = _batchNoes.Any(i => i.TotalNeeded != 0);
                if (isHasBatchItem)
                {
                    return Ok(new { IsBatch = true, Data = _batchNoes });
                }
                using var t = _context.Database.BeginTransaction();
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = Series.NextNo;
                long No = long.Parse(Sno);
                Series.NextNo = Convert.ToString(No + 1);
                issues.SeriseDID = seriesDetailID;
                _context.GoodIssues.Update(issues);
                _context.Series.Update(Series);
                _context.SaveChanges();

                var IssuesID = issues.GoodIssuesID;
                 if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                 {
                _issuse.IssuesStockBasic(IssuesID, issues.CompanyID, serialNumber, batchNoes);

                 }
                 else{
                _issuse.IssuesStock(IssuesID, issues.CompanyID, serialNumber, batchNoes);
                 }
                t.Commit();
                ModelState.AddModelError("success", $"Add item{(issues.GoodIssuesDetails.Count > 1 ? "s" : "")} goods issue successfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindBarcode(int WarehouseID, string Barcode)
        {
            try
            {
                var list = _issuse.GetItemFindBarcode(Barcode);
                if (list == null)
                {
                    return Ok(new
                    {
                        ActiveError = true,
                        Error = $"Barcode \"{Barcode}\" not found!"
                    });
                }
                var itemLists = _issuse.GetAllPurItemsByItemID(WarehouseID, list.ID, GetCompany().ID);
                return Ok(itemLists);
            }
            catch (Exception)
            {

                return Ok();
            }
        }
        #region  ValidatErrorBasic
 private void ValidatErrorBasic(GoodIssues issues)
        {
            if (issues.WarehouseID == 0)
            {
                ModelState.AddModelError("Warehouse", "Please select warehouse !");
            }
            
            if (issues.GoodIssuesDetails == null)
            {
                ModelState.AddModelError("GoodIssuesDetails", "Please choose any items to do goods issue!");
            }
            if (issues.GoodIssuesDetails != null)
            {
                if (!issues.GoodIssuesDetails.Where(i => i.Quantity > 0).Any())
                {
                    ModelState.AddModelError("GoodIssuesDetailsQ", "Please fill any items' quantity to do goods issue!");
                }
            }
        }

        #endregion ValidatErrorBasic
        #region  ValidatError
        private void ValidatError(GoodIssues issues)
        {
            if (issues.WarehouseID == 0)
            {
                ModelState.AddModelError("Warehouse", "Please select warehouse !");
            }
            if (issues.GLID == 0 && issues.GoodIssuesDetails.Any(s => s.GLID == 0))
            {
                ModelState.AddModelError("Warehouse", "Please select Expense Account !");
            }
            if (issues.GoodIssuesDetails == null)
            {
                ModelState.AddModelError("GoodIssuesDetails", "Please choose any items to do goods issue!");
            }
            if (issues.GoodIssuesDetails != null)
            {
                if (!issues.GoodIssuesDetails.Where(i => i.Quantity > 0).Any())
                {
                    ModelState.AddModelError("GoodIssuesDetailsQ", "Please fill any items' quantity to do goods issue!");
                }
                if (issues.GLID == 0)
                {
                    foreach (var (item, index) in issues.GoodIssuesDetails.Select((item, index) => (item, index)))
                    {
                        if (item.GLID == 0)
                        {
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(i => i.ID == item.ItemID) ?? new ItemMasterData();
                            ModelState.AddModelError($"GLAccount{index}", $"Item Name {itemMaster.KhmerName} at line {index + 1} is require Payment Means");
                        }
                    }
                }
            }
        }
        #endregion ValidatError
    }
}
