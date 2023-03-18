using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.BatchNoManagement;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class ItemBatchNoController : Controller
    {
        private readonly DataContext _context;
        private readonly IItemBatchNoRepository _itemBatch;

        public ItemBatchNoController(DataContext context, IItemBatchNoRepository itemBatch)
        {
            _context = context;
            _itemBatch = itemBatch;
        }
        public IActionResult BatchManagement()
        {
            ViewBag.BatchManagement = "highlight"; 
            return View();
        }
        public IActionResult BatchDetails()
        {
            ViewBag.BatchDetails = "highlight";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetBatchNoDoc(string filter)
        {
            var _filter = JsonConvert.DeserializeObject<SNFilter>(filter, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            var item = await _context.ItemMasterDatas
                .FirstOrDefaultAsync(i => 
                    i.Code.ToLower() == _filter.ItemNo.ToLower() && 
                    i.ManItemBy == ManageItemBy.Batches);

            if (item == null)
            {
                ModelState.AddModelError("Item", "Item not found!");
                return Ok(msg.Bind(ModelState));
            }
            var data = await _itemBatch.GetBatchNoDocAsync(_filter, item);
            return Ok(data);
        }

        public async Task<IActionResult> GetItemMasterData()
        {
            var data = await _itemBatch.GetItemMasterDataAsync();
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetBatchCreated(int itemId, int saleId, TransTypeWD transType)
        {
            var data = _itemBatch.GetBatchCreated(itemId, saleId, transType);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetBatchCreatedWPId(string propId, int itemId, int saleId, TransTypeWD transType)
        {
            var data = _itemBatch.GetBatchCreated(propId, itemId, saleId, transType);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult UpdateBatchNo(List<BatchNoDocumentViewModel> batches)
        {
            ModelMessage msg = new();
            CheckBatch(batches);
            if (ModelState.IsValid)
            {
                try
                {
                    _itemBatch.UpdateBatchNo(batches);
                    ModelState.AddModelError("success", "Items were updated!");
                    msg.Approve();
                }
                catch
                {
                    ModelState.AddModelError("fails", "Please try again or contact our support!");
                    msg.Reject();
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult UpdateBatchNoDetial(BatchNoDetialViewModel batchDetail)
        {
            ModelMessage msg = new();
            CheckBatchNoDetial(batchDetail);
            if (ModelState.IsValid)
            {
                try
                {
                    _itemBatch.UpdateBatchNoDetial(batchDetail);
                    ModelState.AddModelError("success", "Items were updated!");
                    msg.Approve();
                }
                catch
                {
                    ModelState.AddModelError("fails", "Please try again or contact our support!");
                    msg.Reject();
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult GetBatchNoDetails(string itemcode)
        {
            ModelMessage msg = new();
            var item = _context.ItemMasterDatas.FirstOrDefault(i => i.Code.ToLower() == itemcode.ToLower() && i.ManItemBy == ManageItemBy.Batches);
            if (item == null)
            {
                ModelState.AddModelError("item", $"Do not have item with code \"{itemcode}\"");
                return Ok(msg.Bind(ModelState));
            }
            var data = _itemBatch.GetBatchNoDetails(itemcode);
            return Ok(data);
        }
        private void CheckBatch(List<BatchNoDocumentViewModel> batches)
        {
            //serials = serials.Where(i => i.Serials.Count > 0).ToList();
            foreach (var batch in batches.Select((value, index) => (value, index)))
            {
                if (batch.value.Batches.Count > 0)
                {
                    foreach (var batchDetail in batch.value.Batches.Select((value, index) => (value, index)))
                    {
                        var warehouseDetial = _context.WarehouseDetails.FirstOrDefault(i => i.BatchNo == batchDetail.value.Batch);

                        if (warehouseDetial != null && batchDetail.value.Batch != batchDetail.value.BatchOG)
                        {
                            //_logger.LogInformation($"{serial.index}");
                            ModelState.AddModelError(
                                $"{batch.value.ItemCode}{batch.value.LineID}",
                                $"Item code \"{batch.value.ItemCode}\" at line \"{batch.index + 1}\" Batch No \"{batchDetail.value.Batch}\" at line \"{batchDetail.index + 1}\" already existed"
                            );
                        }
                    }
                }
            }
        }
        private void CheckBatchNoDetial(BatchNoDetialViewModel batchDetail)
        {
            var warehouseDetial = _context.WarehouseDetails
                .FirstOrDefault(i => i.BatchNo == batchDetail.Batch);
            if (warehouseDetial != null && batchDetail.Batch != batchDetail.BatchOG)
            {
                ModelState.AddModelError("ItemBatch", $"Batch No {batchDetail.Batch} already existed");
            }
        }
    }
}
