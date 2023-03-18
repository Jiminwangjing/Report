using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class ItemSerialNumbersController : Controller
    {
        private readonly IItemSerialNumberRepository _itemSerial;
        private readonly DataContext _context;
        private readonly ILogger<ItemSerialNumbersController> _logger;
        public ItemSerialNumbersController(DataContext context, IItemSerialNumberRepository itemSerial, ILogger<ItemSerialNumbersController> logger)
        {
            _itemSerial = itemSerial;
            _context = context;
            _logger = logger;
        }
        public IActionResult SerialNumberManagement()
        {
            ViewBag.SerialNumberManagement = "highlight";
            return View();
        }
        public IActionResult SerialNumberDetails()
        {
            ViewBag.SerialNumberDetails = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult GetCreatedSerials(int itemId, int saleId, TransTypeWD transType)
        {
            var data = _itemSerial.GetCreatedSerials(itemId, saleId, transType);
            return Ok(data);
        }
        public IActionResult GetCreatedSerialsWPId(string propId, int itemId, int saleId, TransTypeWD transType)
        {
            var data = _itemSerial.GetCreatedSerials(propId, itemId, saleId, transType);
            return Ok(data);
        }

        public async Task<IActionResult> GetItemMasterData()
        {
            var data = await _itemSerial.GetItemMasterDataAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSerialNumberDoc(string filter)
        {
            var _filter = JsonConvert.DeserializeObject<SNFilter>(filter, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            var item = await _context.ItemMasterDatas.FirstOrDefaultAsync(i => i.Code.ToLower() == _filter.ItemNo.ToLower() && i.ManItemBy == ManageItemBy.SerialNumbers);
            if (item == null)
            {
                ModelState.AddModelError("Item", "Item not found!");
                return Ok(msg.Bind(ModelState));
            }
            var data = await _itemSerial.GetSerialNumberDocAsync(_filter, item);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult UpdateSerialNumber(List<SerialNumberDocumentViewModel> serials)
        {
            ModelMessage msg = new();
            CheckSerail(serials);
            if (ModelState.IsValid)
            {
                try
                {
                    _itemSerial.UpdateSerialNumber(serials);
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
        public IActionResult UpdateSerialNumberDetial(ItemSerialNumberDetialView serialDetail)
        {
            ModelMessage msg = new();
            CheckSerailDetial(serialDetail);
            if (ModelState.IsValid)
            {
                try
                {
                    _itemSerial.UpdateSerialNumberDetial(serialDetail);
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
        public IActionResult GetSerialItemDetails(string itemcode)
        {
            ModelMessage msg = new();
            var item = _context.ItemMasterDatas.FirstOrDefault(i => i.Code.ToLower() == itemcode.ToLower() && i.ManItemBy == ManageItemBy.SerialNumbers);
            if(item == null)
            {
                ModelState.AddModelError("item", $"Do not have item with code \"{itemcode}\"");
                return Ok(msg.Bind(ModelState));
            }
            var data = _itemSerial.GetSerialItemDetails(itemcode);
            return Ok(data);
        }
        private void CheckSerail(List<SerialNumberDocumentViewModel> serials)
        {
            //serials = serials.Where(i => i.Serials.Count > 0).ToList();
            foreach(var serial in serials.Select((value, index) => (value, index)))
            {
                if(serial.value.Serials.Count > 0)
                {
                    foreach (var serialDetail in serial.value.Serials.Select((value, index) => (value, index)))
                    {
                        var warehouseDetial = _context.WarehouseDetails.FirstOrDefault(i => i.SerialNumber == serialDetail.value.SerialNumber);

                        if (warehouseDetial != null && serialDetail.value.SerialNumber != serialDetail.value.SerialNumberOG)
                        {
                            //_logger.LogInformation($"{serial.index}");
                            ModelState.AddModelError(
                                $"{serial.value.ItemCode}{serial.value.LineID}",
                                $"Item code \"{serial.value.ItemCode}\" at line \"{serial.index + 1}\" Serial Number \"{serialDetail.value.SerialNumber}\" at line \"{serialDetail.index + 1}\" already existed"
                            );
                        }
                    }
                }
            }
        }
        private void CheckSerailDetial(ItemSerialNumberDetialView serialDetail)
        {
            var warehouseDetial = _context.WarehouseDetails
                .FirstOrDefault(i => i.SerialNumber == serialDetail.SerialNumber);
            if (warehouseDetial != null && serialDetail.SerialNumber != serialDetail.SerialNumberOG)
            {
                ModelState.AddModelError("ItemSerial",$"Serial Number {serialDetail.SerialNumber} already existed");
            }
        }
    }
}
