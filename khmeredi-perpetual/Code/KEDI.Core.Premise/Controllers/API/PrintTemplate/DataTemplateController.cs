using CKBS.AppContext;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers.API.PrintTemplate
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class DataTemplateController : ControllerBase
    {
        private readonly IPrintTemplateRepository _printTemplate;

        public DataTemplateController(IPrintTemplateRepository printTemplate)
        {
            _printTemplate = printTemplate;
        }

        [Route("{id}")]
        public IActionResult SaleARHistory(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);

        }
        [Route("{purcahseId}")]
        public IActionResult PrintPurchaseAP(int purcahseId)
        {
            var list = _printTemplate.GetPrintPurchaseAP(purcahseId);
            return Ok(list);
        }
        [Route("{purchaseid}")]
        public IActionResult PrintPurchaseAPReserve(int purchaseid)
        {
            var list = _printTemplate.GetPrintPurchaseAPResers(purchaseid);
            return Ok(list);
        }
        [Route("{purchaseid}")]
        public IActionResult PrintPurchaseQuotation(int purchaseid)
        {
            var list = _printTemplate.GetPurchaseQuotation(purchaseid);
            return Ok(list);
        }
        [Route("{purchaseId}")]
        public IActionResult PrintPurchaseRequest(int purchaseId)
        {
            var list = _printTemplate.GetPrintPurchaseRequest(purchaseId);
            return Ok(list);
        }
        [Route("{purchaseid}")]
        public IActionResult PrintPurchaseMemo(int purchaseid)
        {
            var list = _printTemplate.GetPrintPurchaseCreditmemo(purchaseid);
            return Ok(list);
        }
        [HttpGet("{id}")]
        public IActionResult SaleARReserveInvoice(int id)
        {
            var list = _printTemplate.GetSaleARReserve(id);
            return Ok(list);
        }
        [HttpGet("{id}")]
        public IActionResult ARReserveEditHistory(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult ARReserveEdit01History(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult ARReserveHistoryVatC(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult SaleAReserveNoneVat01History(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult SaleARReserveHistoryVat(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult ARReserveHistoryVatB(int id)
        {
            var list = _printTemplate.GetSaleARReserveEdit(id);
            return Ok(list);
        }
        public IActionResult SaleARHistoryTp01(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }
        public IActionResult SaleARHistoryTp02(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }
        public IActionResult SaleARHistoryTp03(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }

        public IActionResult SaleARHistoryTp04(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }

        public IActionResult SaleARHistoryTp05(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }

        public IActionResult SaleARHistoryNoneVat(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);
        }
         [Route("{id}")]
         public IActionResult SaleARRINVHistory(int id)// SaleARReserve Invoice
        {
            var list = _printTemplate.GetSaleARReserve(id);
            return Ok(list);
        }
        public IActionResult SaleARHistoryVat(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);
        }
        [Route("{id}")]
        public IActionResult PrintPreviewPOS(int id)
        {
            var list = _printTemplate.GetPOSData(id);
            return Ok(list);
        }
        [Route("{id}/{userid}")]
        public async Task<IActionResult> RePrintPreviewPOS(int id, int userid)
        {
            var list = await _printTemplate.GetRePrintPOSData(id, userid);
            return Ok(list);
        }
        public IActionResult SaleARHistoryVatB(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);
        }
        public IActionResult SaleARHistoryVatBC(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);
        }
        public IActionResult SaleARNoneVat01History(int id)
        {
            var list = _printTemplate.GetSaleARData(id);
            return Ok(list);
        }
        [HttpGet("{id}")]
        public IActionResult SaleARHistoryEdit(int id)
        {
            var list = _printTemplate.GetSaleARDataEdit(id);
            return Ok(list);
        }

        [Route("{id}")]
        public IActionResult SaleQuoteHistory(int id)
        {
            var list = _printTemplate.GetSaleQoute(id);
            return Ok(list);
        }
        [Route("{id}")]
        public IActionResult SaleOrderHistory(int id)
        {
            var list = _printTemplate.GetSaleOrderHistory(id);
            return Ok(list);
        }
        [Route("{id}")]
        public IActionResult SaleDelivery(int id)
        {
            var list = _printTemplate.GetSaleDelivery(id);
            return Ok(list);
        }
        [Route("{id}")]
        public IActionResult ReturnDelivery(int id)
        {
            var list = _printTemplate.GetReturnDeilivery(id);
            return Ok(list);
        }
        [Route("{id}")]
        public IActionResult SaleArDownPaymentHistory(int id)
        {
            var list = _printTemplate.GetSaleARDownPaymentHistory(id);
            return Ok(list);
        }
        [Route("{purchaseid}")]
        public IActionResult PurchaseOrder(int purchaseid)
        {
            var list = _printTemplate.GetPurchaseOrder(purchaseid);
            return Ok(list);
        }
        [Route("{purchaseid}")]
        public IActionResult PurchasePO(int purchaseid)
        {
            var list = _printTemplate.GetPurchasePO(purchaseid);
            return Ok(list);
        }

        [Route("{id}")]
        public IActionResult SaleCreditmemo(int id)
        {
            var list = _printTemplate.GetSaleCreditmemo(id);
            return Ok(list);
        }

        [Route("{dateFrom}/{dateTo}/{branchID}/{userID}/{timeFrom}/{timeTo}/{plid}/{doctype}")]
        public IActionResult PrintSummarySale(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo, int plid, string doctype)
        {
            var data = _printTemplate.GetSaleSummary(dateFrom, dateTo, branchID, userID, timeFrom, timeTo, plid, doctype);
            return Ok(data);
        }

        [Route("{dateFrom}/{dateTo}/{timeFrom}/{timeTo}/{branchID}/{userID}/{douType}")]
        public IActionResult PrintCreditMemoReport(string dateFrom, string dateTo, string timeFrom, string timeTo, int branchID, int userID, string douType)
        {
            var data = _printTemplate.GetSaleCrditMemoReport(dateFrom, dateTo, timeFrom, timeTo, branchID, userID, douType);
            return Ok(data);
        }
    }
}
