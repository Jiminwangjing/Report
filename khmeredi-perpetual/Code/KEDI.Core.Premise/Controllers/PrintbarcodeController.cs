using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class PrintbarcodeController : Controller
    {
        private readonly IPrintBarcodeRepository _printBarcode;
        public PrintbarcodeController(IPrintBarcodeRepository printBarcode)
        {
            _printBarcode = printBarcode;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PrinterNameModel barcodeForm)
        {
            var itemBarcodes = barcodeForm.ItemPrintBarcodes.Where(p => p.IsSelected).ToList();
            itemBarcodes.ForEach(b => {
                b.ItemDes = string.IsNullOrWhiteSpace(b.ItemDes) ? " " : b.ItemDes;
            });
            return View(itemBarcodes);
        }

        public async Task<IActionResult> Barcodelayout()
        {
            ViewBag.Printbarcode = "highlight";
            var pls = await _printBarcode.GetPriceListsAsync();
            ViewBag.PriceLists = new SelectList(pls, "ID", "Name");
            return View(new PrinterNameModel
            {
                ItemPrintBarcodes = new List<ItemPrintBarcodeView>()
            });
        }

        public async Task<IActionResult> GetItemMasterData(int plId, int count, string setting)
        {
            var data = await _printBarcode.GetItemMasterBaseOnPriceListAsync(plId, count, setting);
            return Ok(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BarLabel(PrinterNameModel barcodeForm)
        {            
            return View(barcodeForm.ItemPrintBarcodes.Where(p => p.IsSelected).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> BarLabelLayout()
        {
            ViewBag.BarLabelLayout = "highlight";
            var pls = await _printBarcode.GetPriceListsAsync();
            ViewBag.PriceLists = new SelectList(pls, "ID", "Name");
            return View(new PrinterNameModel
            {
                ItemPrintBarcodes = new List<ItemPrintBarcodeView>()
            });
        }
    }
}
