using CKBS.AppContext;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class BarcodeToServiceController : Controller
    {
        private readonly IPrintBarcodeRepository _printBarcode;
        private readonly DataContext _barcode;
        public BarcodeToServiceController(IPrintBarcodeRepository printBarcode, DataContext barcode)
        {
            _printBarcode = printBarcode;
            _barcode = barcode;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.BarcodePrinter = "highlight";
            var printers = await _printBarcode.GetPrinterNames();
            var pls = await _printBarcode.GetPriceListsAsync();
            ViewBag.Printer = new SelectList(printers,"Name", "Name");
            ViewBag.PriceList = new SelectList(pls,"ID", "Name");
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(PrinterNameModel data)
        {
            await _printBarcode.PrintBarcodeItemsAsync(data.ItemPrintBarcodes, data.Printername);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetItemMasterData(int plId,int count,string setting)
        {
            var data = await _printBarcode.GetItemMasterBaseOnPriceListAsync(plId,count, setting);
            return Ok(data);
        }
    }
}
