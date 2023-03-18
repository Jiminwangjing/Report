using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.ServicesClass;
using KEDI.Core.Models.Validation;
using Newtonsoft.Json;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.Responsitory;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Repository;
using KEDI.Core.System.Models;
using Microsoft.EntityFrameworkCore;

namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class GoodsReceiptController : Controller
    {
        private readonly DataContext _context;
        private readonly IGoodsReceipt _receipt;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _utilityModule;

        public GoodsReceiptController(DataContext context, IGoodsReceipt receipt, IPurchaseRepository ipur, UtilityModule utilityModule)
        {
            _context = context;
            _receipt = receipt;
            _ipur = ipur;
            _utilityModule = utilityModule;
        }
        [HttpGet]
        [Privilege("A039")]
        public IActionResult GoodsReceipt()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Transaction";
            ViewBag.Subpage = "Goods Receipt";
            ViewBag.InventoryMenu = "show";
            ViewBag.Transaction = "show";
            ViewBag.GoodsReceipt = "highlight";
            return View(new { seriesGR = _utilityModule.GetSeries("GR"), seriesJE = _utilityModule.GetSeries("JE") });
        }

        [HttpGet]
        public IActionResult GetWarehousesFrom(int ID)
        {
            var list = _receipt.GetWarehouse_From(ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemByWarehouseFrom(int ID)
        {
            var list = _receipt.GetItemByWarehouse_From(ID, GetCompany().ID).ToList();
            return Ok(list);
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
        [HttpPost]
        public IActionResult SaveGoodsReceipt(string data, string serial = null, string batch = null)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            GoodsReceipt goodsReceipt = JsonConvert.DeserializeObject<GoodsReceipt>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "GR");
            var Series = _context.Series.Find(goodsReceipt.SeriseID);
            seriesDetail.Number = Series.NextNo;
            seriesDetail.SeriesID = Series.ID;
            goodsReceipt.UserID = GetUserID();
            goodsReceipt.CompanyID = GetCompany().ID;
            goodsReceipt.SysCurID = GetCompany().SystemCurrencyID;
            goodsReceipt.LocalCurID = GetCompany().LocalCurrencyID;
            goodsReceipt.Number_No = seriesDetail.Number;
            goodsReceipt.DocTypeID = douType.ID;
            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                ValidationBasic(goodsReceipt);
            }
            else
            {
                Validation(goodsReceipt);
            }


            if (ModelState.IsValid)
            {
                // Checking Serial Batch //
                List<SerialViewModelPurchase> serialViewModelPurchases = new();
                List<SerialViewModelPurchase> _serialViewModelPurchases = new();
                List<BatchViewModelPurchase> batchViewModelPurchases = new();
                List<BatchViewModelPurchase> _batchViewModelPurchases = new();

                serialViewModelPurchases = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialViewModelPurchase>>(serial, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialViewModelPurchases;

                _ipur.CheckItemSerailGoodsReceipt(goodsReceipt, serialViewModelPurchases);
                serialViewModelPurchases = serialViewModelPurchases
                    .GroupBy(i => new { i.ItemID, i.Cost })
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialViewModelPurchases.ToList())
                {
                    foreach (var i in goodsReceipt.GoodReceiptDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)i.Cost)
                        {
                            _serialViewModelPurchases.Add(j);
                        }
                    }
                }
                bool isHasSerialItem = _serialViewModelPurchases.Any(i => i.TotalCreated != i.TotalNeeded);
                if (isHasSerialItem)
                {
                    return Ok(new { IsSerail = true, Data = _serialViewModelPurchases });
                }
                // checking batch items
                batchViewModelPurchases = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchViewModelPurchase>>(batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchViewModelPurchases;
                _ipur.CheckItemBatchGoodsReceipt(goodsReceipt, batchViewModelPurchases);
                batchViewModelPurchases = batchViewModelPurchases
                    .GroupBy(i => new { i.ItemID, i.Cost })
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchViewModelPurchases.ToList())
                {
                    foreach (var i in goodsReceipt.GoodReceiptDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)i.Cost)
                        {
                            _batchViewModelPurchases.Add(j);
                        }
                    }
                }
                bool isHasBatchItems = _batchViewModelPurchases.Any(i => i.TotalNeeded != i.TotalCreated);
                if (isHasBatchItems)
                {
                    return Ok(new { IsBatch = true, Data = _batchViewModelPurchases });
                }
                using var t = _context.Database.BeginTransaction();
                var lc = _context.ExchangeRates.AsNoTracking().FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID) ?? new ExchangeRate();
                // insert Series Detail
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = Series.NextNo;
                long No = long.Parse(Sno);
                Series.NextNo = Convert.ToString(No + 1);
                goodsReceipt.SeriseDID = seriesDetailID;
                goodsReceipt.LocalSetRate = lc.SetRate;
                goodsReceipt.GoodReceiptDetails = goodsReceipt.GoodReceiptDetails.Where(i => i.Quantity > 0).ToList();
                _context.GoodsReceipts.Update(goodsReceipt);
                _context.Series.Update(Series);
                _context.SaveChanges();
                var IssuesID = goodsReceipt.GoodsReceiptID;
                if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                {
                    _receipt.SaveGoodIssuesBasic(IssuesID, serialViewModelPurchases, batchViewModelPurchases);
                }
                else
                {
                    _receipt.SaveGoodIssues(IssuesID, serialViewModelPurchases, batchViewModelPurchases);
                }
                t.Commit();
                ModelState.AddModelError("success", $"Add item{(goodsReceipt.GoodReceiptDetails.Count > 1 ? "s" : "")} goods receipt successfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        #region  ValidationBasic
        private void ValidationBasic(GoodsReceipt goodsReceipt)
        {
            if (goodsReceipt.WarehouseID == 0)
            {
                ModelState.AddModelError("Warehouse", "Please select warehouse !");
            }
            if (goodsReceipt.GoodReceiptDetails == null)
            {
                ModelState.AddModelError("GoodsReceiptDetail", "Please choose any items to do goods receipt!");
            }
            if (goodsReceipt.GoodReceiptDetails != null)
            {
                if (!goodsReceipt.GoodReceiptDetails.Where(i => i.Quantity > 0).Any())
                {
                    ModelState.AddModelError("GoodsReceiptDetailQ", "Please fill any items' quantity to do goods receipt!");
                }

            }
        }

        #endregion ValidationBasic
        #region  Validation
        private void Validation(GoodsReceipt goodsReceipt)
        {
            if (goodsReceipt.WarehouseID == 0)
            {
                ModelState.AddModelError("Warehouse", "Please select warehouse !");
            }
            if (goodsReceipt.GLID == 0 && goodsReceipt.GoodReceiptDetails.Any(s => s.GLID == 0))
            {
                ModelState.AddModelError("Warehouse", "Please select Expense Account !");
            }
            if (goodsReceipt.GoodReceiptDetails == null)
            {
                ModelState.AddModelError("GoodsReceiptDetail", "Please choose any items to do goods receipt!");
            }
            if (goodsReceipt.GoodReceiptDetails != null)
            {
                if (!goodsReceipt.GoodReceiptDetails.Where(i => i.Quantity > 0).Any())
                {
                    ModelState.AddModelError("GoodsReceiptDetailQ", "Please fill any items' quantity to do goods receipt!");
                }
                if (goodsReceipt.GLID == 0)
                {
                    foreach (var (item, index) in goodsReceipt.GoodReceiptDetails.Select((item, index) => (item, index)))
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
        #endregion Validation
        [HttpGet]
        public IActionResult FindBarcode(int WarehouseID, string Barcode)
        {
            try
            {
                var list = _receipt.GetItemFindeBarcode(Barcode);
                if (list == null)
                {
                    return Ok(new
                    {
                        ActiveError = true,
                        Error = $"Barcode \"{Barcode}\" not found!"
                    });
                }
                var itemLists = _receipt.GetAllPurItemsByItemID(WarehouseID, list.ID, GetCompany().ID);
                return Ok(itemLists);
            }
            catch (Exception)
            {

                return Ok();
            }
        }
        [HttpGet]
        public IActionResult GetPaymentMeans()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                select new
                                {
                                    pm.ID,
                                    PMName = pm.Type,
                                    GLAccName = glAcc.Name,
                                    GLAccCode = glAcc.Code,
                                }
                               ).ToList();
            return Ok(paymentMeans);
        }
        [HttpGet]
        public IActionResult GetPaymentMeansDefault()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => i.Default == true && !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                select new
                                {
                                    pm.ID,
                                    PMName = pm.Type,
                                    GLAccName = glAcc.Name,
                                    GLAccCode = glAcc.Code,
                                }
                                ).ToList();
            return Ok(paymentMeans);
        }
        public IActionResult GetAllPurItemsByItemID(int wareid, int itemID)
        {
            var items = _receipt.GetAllPurItemsByItemID(wareid, itemID, GetCompany().ID);
            return Ok(items);
        }
    }
}
