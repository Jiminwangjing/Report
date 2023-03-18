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
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Newtonsoft.Json;
using KEDI.Core.Premise.Models.Services.Responsitory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Repository;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.Inventory.Transaction;
using Microsoft.EntityFrameworkCore;

namespace CKBS.Controllers
{
    [Privilege]
    public class TransferController : Controller
    {
        private readonly DataContext _context;
        public readonly ITransfer _transfer;
        private readonly ISaleSerialBatchRepository _saleSerialBatch;
        private readonly UtilityModule _utility;
        public TransferController(DataContext context, ITransfer transfer, ISaleSerialBatchRepository saleSerialBatch, UtilityModule utility)
        {
            _transfer = transfer;
            _context = context;
            _saleSerialBatch = saleSerialBatch;
            _utility = utility;
        }

        [Privilege("A041")]
        public IActionResult Transfer()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Transaction";
            ViewBag.Subpage = "Transfer";
            ViewBag.InventoryMenu = "show";
            ViewBag.Transaction = "show";
            ViewBag.Transfer = "highlight";
            return View(new { seriesST = _utility.GetSeries("ST"), seriesJE = _utility.GetSeries("JE") });
        }

        public IActionResult TransferRequest()
        {
            ViewBag.style = "fa fa-cubes";
            ViewBag.Main = "Inventory";
            ViewBag.Page = "Transaction";
            ViewBag.Subpage = "Transfer Request";
            ViewBag.InventoryMenu = "show";
            ViewBag.Transaction = "show";
            ViewBag.TransferRequest = "highlight";
            return View(new { seriesST = _utility.GetSeries("TR"), seriesJE = _utility.GetSeries("JE") });
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
        public IActionResult GetWarehousesFrom(int BranchID)
        {
            var list = _transfer.GetFromWarehouse(BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehousesTo(int BranchID)
        {
            var list = _transfer.GetToWarehouse.Where(x => x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBranch()
        {
            var list = _transfer.GetBranches.ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouse_By_filterBranch(int BranchID)
        {
            var list = _transfer.GetWarehouse_filter_Branch(BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetItemByWarehouseFrom(int warehouesId)
        {
            var list = _transfer.GetItemMasterBy_Warehouse(warehouesId, GetCompany().ID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetAllPurItemsByItemID(int wareId, int ID)
        {
            var list = _transfer.GetAllPurItemsByItemID(wareId, ID, GetCompany().ID);
            return Ok(list);
        }

        public IActionResult SaveTransfer(Transfer transfer, string serial, string batch)
        {
            ModelMessage msg = new();
            // Checking Serial Batch //
            List<SerialNumber> serialNumber = new();
            List<SerialNumber> _serialNumber = new();
            List<BatchNo> batchNoes = new();
            List<BatchNo> _batchNoes = new();
            serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : serialNumber;

            _saleSerialBatch.CheckItemSerailTransfer(transfer, transfer.TransferDetails, serialNumber);
            serialNumber = serialNumber
                .GroupBy(i => new { i.ItemID, i.Cost })
                .Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in serialNumber.ToList())
            {
                foreach (var i in transfer.TransferDetails.ToList())
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
            _saleSerialBatch.CheckItemBatchTransfer(transfer, transfer.TransferDetails, batchNoes);
            batchNoes = batchNoes
                .GroupBy(i => new { i.ItemID, i.Cost })
                .Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in batchNoes.ToList())
            {
                foreach (var i in transfer.TransferDetails.ToList())
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
            ErrorSummary(transfer);
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                if (transfer.UserRequestID == 0)
                {
                    transfer.UserRequestID = transfer.UserID;
                }
                if (transfer.Time == null)
                {
                    transfer.Time = DateTime.Now.ToShortTimeString();
                }
                var warehFrom = _context.Warehouses.Find(transfer.WarehouseFromID);
                var warehTo = _context.Warehouses.Find(transfer.WarehouseToID);
                var lc = _context.ExchangeRates.Where(w => w.CurrencyID == GetCompany().LocalCurrencyID);
                SeriesDetail seriesDetail = new();
                var DouType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "ST");
                var Series = _context.Series.Find(transfer.SeriseID);
                // insert Series Detail
                seriesDetail.Number = Series.NextNo;
                seriesDetail.SeriesID = Series.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = Series.NextNo;
                long No = long.Parse(Sno);
                Series.NextNo = Convert.ToString(No + 1);
                transfer.CompanyID = GetCompany().ID;
                transfer.SysCurID = GetCompany().SystemCurrencyID;
                transfer.LocalCurID = GetCompany().LocalCurrencyID;
                transfer.LocalSetRate = lc.FirstOrDefault().SetRate;
                transfer.SeriseID = Series.ID;
                transfer.SeriseDID = seriesDetailID;
                transfer.Number = seriesDetail.Number;
                transfer.DocTypeID = DouType.ID;
                _context.Transfers.Update(transfer);
                _context.Series.Update(Series);
                _context.SaveChanges();
                var TransferID = transfer.TarmsferID;
                if (transfer.BaseOnID > 0)
                    _transfer.UpdateTransferRequest(transfer.TransferDetails, transfer.BaseOnID);
                _transfer.SaveTrasfers(TransferID, serialNumber, batchNoes);
                t.Commit();
                ModelState.AddModelError("success",
                    $"Transfer item" +
                    $"{(transfer.TransferDetails.Count > 1 ? "s" : "")} " +
                    $"from warehouse \"{warehFrom.Name}\" to warehouse \"{warehTo.Name}\" successfully!"
                    );
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet]
        public IActionResult FindBarcode(int WarehouseID, string Barcode)
        {
            try
            {
                var list = _transfer.GetItemFindBarcode(WarehouseID, Barcode).FirstOrDefault();
                if (list == null)
                {
                    return Ok(new
                    {
                        ActiveError = true,
                        Error = $"Barcode \"{Barcode}\" not found!"
                    });
                }
                var itemLists = _transfer.GetAllPurItemsByItemID(WarehouseID, list.ItemID, GetCompany().ID);
                return Ok(itemLists);
            }
            catch (Exception)
            {

                return Ok();
            }
        }

        private void ErrorSummary(Transfer transfer)
        {
            if (transfer.BranchID == 0)
            {
                ModelState.AddModelError("BranchID", "Please select branch!");
            }
            if (transfer.WarehouseFromID == 0)
            {
                ModelState.AddModelError("WHF", "Please select warehouse from!");
            }
            if (transfer.WarehouseToID == 0)
            {
                ModelState.AddModelError("WHT", "Please select warehouse to!");
            }
            if (transfer.TransferDetails == null)
            {
                ModelState.AddModelError("TransferDetails", "Please choose any items!");
            }
            if (transfer.TransferDetails != null)
            {
                foreach (var (item, index) in transfer.TransferDetails.Select((item, index) => (item, index)))
                {
                    if (item.FWarehouseID > 0 && item.TWarehouseID > 0)
                    {
                        if (item.FWarehouseID == item.TWarehouseID)
                        {
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(i => i.ID == item.ItemID) ?? new ItemMasterData();
                            ModelState.AddModelError($"GLAccount{index}", $"Item Name {itemMaster.KhmerName} at line {index + 1} same warehouse!");
                        }
                    }
                }
                if (transfer.TransferDetails.Where(i => i.Qty > 0).ToList().Count == 0)
                {
                    ModelState.AddModelError("TransferDetails", "Please fill any items' QTY!");
                }
                if (transfer.WarehouseFromID == transfer.WarehouseToID)
                {
                    ModelState.AddModelError("WHC", $"Can not transfer item{(transfer.TransferDetails.Where(i => i.Qty > 0).Count() > 1 ? "s" : "")} to the same warehouse!");
                }
            }
            if (transfer.WarehouseFromID == transfer.WarehouseToID)
            {
                ModelState.AddModelError("WHC", "Can not transfer items to the same warehouse!");
            }
        }
        /// start Request from 


        public IActionResult SaveTransferRequest(TransferRequest transfer)
        {
            ModelMessage msg = new();
            if (transfer.BranchID == 0)
            {
                ModelState.AddModelError("BranchID", "Please select branch!");
            }
            if (transfer.WarehouseFromID == 0)
            {
                ModelState.AddModelError("WHF", "Please select warehouse from!");
            }
            if (transfer.WarehouseToID == 0)
            {
                ModelState.AddModelError("WHT", "Please select warehouse to!");
            }
            if (transfer.TransferRequestDetails.Count == 0)
            {
                ModelState.AddModelError("TransferDetails", "Please choose any items!");
            }
            else
            {
                foreach (var (item, index) in transfer.TransferRequestDetails.Select((item, index) => (item, index)))
                {
                    item.OPenQty = item.Qty;
                    if (item.FWarehouseID > 0 && item.TWarehouseID > 0)
                    {
                        if (item.FWarehouseID == item.TWarehouseID)
                        {
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(i => i.ID == item.ItemID) ?? new ItemMasterData();
                            ModelState.AddModelError($"GLAccount{index}", $"Item Name {itemMaster.KhmerName} at line {index + 1} same warehouse!");
                        }
                    }
                }
                if (transfer.TransferRequestDetails.Where(i => i.Qty > 0).ToList().Count == 0)
                {
                    ModelState.AddModelError("TransferDetails", "Please fill any items' QTY!");
                }
                if (transfer.WarehouseFromID == transfer.WarehouseToID)
                {
                    ModelState.AddModelError("WHC", $"Can not transfer item{(transfer.TransferRequestDetails.Where(i => i.Qty > 0).Count() > 1 ? "s" : "")} to the same warehouse!");
                }
            }
            if (transfer.WarehouseFromID == transfer.WarehouseToID)
            {
                ModelState.AddModelError("WHC", "Can not transfer items to the same warehouse!");
            }
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                if (transfer.UserRequestID == 0)
                {
                    transfer.UserRequestID = transfer.UserID;
                }
                if (transfer.Time == null)
                {
                    transfer.Time = DateTime.Now.ToShortTimeString();
                }
                var warehFrom = _context.Warehouses.Find(transfer.WarehouseFromID);
                var warehTo = _context.Warehouses.Find(transfer.WarehouseToID);
                var lc = _context.ExchangeRates.Where(w => w.CurrencyID == GetCompany().LocalCurrencyID);
                SeriesDetail seriesDetail = new();
                var DouType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "TR");
                var Series = _context.Series.Find(transfer.SeriseID);
                // insert Series Detail
                if (transfer.ID == 0)
                {


                    seriesDetail.Number = Series.NextNo;
                    seriesDetail.SeriesID = Series.ID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = Series.NextNo;
                    long No = long.Parse(Sno);
                    Series.NextNo = Convert.ToString(No + 1);

                    transfer.SeriseID = Series.ID;
                    transfer.SeriseDID = seriesDetailID;
                    transfer.Number = seriesDetail.Number;
                }
                transfer.CompanyID = GetCompany().ID;
                transfer.SysCurID = GetCompany().SystemCurrencyID;
                transfer.LocalCurID = GetCompany().LocalCurrencyID;
                transfer.LocalSetRate = lc.FirstOrDefault().SetRate;
                transfer.TranRequStatus = TranRequStatus.Open;
                transfer.DocTypeID = DouType.ID;
                _context.TransferRequests.Update(transfer);
                _context.Series.Update(Series);
                _context.SaveChanges();

                t.Commit();
                ModelState.AddModelError("success",
                    $"Transfer item" +
                    $"{(transfer.TransferRequestDetails.Count > 1 ? "s" : "")} " +
                    $"from warehouse \"{warehFrom.Name}\" to warehouse \"{warehTo.Name}\" successfully!"
                    );
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public async Task<IActionResult> FindTransferRequest(string number)
        {
            var obj = await _transfer.FindTransferRequest(number);
            return Ok(obj);
        }

        public async Task<IActionResult> GetTransferRequest()
        {
            var obj = await (from tr in _context.TransferRequests.Where(s => s.TranRequStatus == TranRequStatus.Open)
                             join war in _context.Warehouses on tr.WarehouseFromID equals war.ID
                             join doc in _context.DocumentTypes on tr.DocTypeID equals doc.ID
                             select new
                             {
                                 ID = tr.ID,
                                 Doctype = doc.Name,
                                 Number = tr.Number,
                                 WarehouseFromID = tr.WarehouseFromID,
                                 Whname = war.Name,
                                 PostingDate = tr.PostingDate.ToString("dd-MM-yyyy"),
                                 Time = tr.Time,

                             }).ToListAsync();
            return Ok(obj);
        }
        public async Task<IActionResult> CopyTransferRequest(string number)
        {
            var obj = await _transfer.FindTransferRequest(number);
            obj.TransferViewModels = obj.TransferViewModels.Where(s => s.OpenQty > 0).ToList();
            obj.TransferViewModels.ForEach(i =>
            {
                i.Qty = i.OpenQty;
            });
            return Ok(obj);
        }

    }
}
