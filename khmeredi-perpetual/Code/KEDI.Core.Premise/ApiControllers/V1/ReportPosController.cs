using CKBS.Models.ServicesClass.Report;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository.Report;
using KEDI.Core.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.ApiControllers.V1
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    [JwtAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportPosController : Controller
    {
        readonly ReportSaleRepo _reportSaleRepo;
        public ReportPosController(ReportSaleRepo reportSaleRepo)
        {
            _reportSaleRepo = reportSaleRepo;
        }

        [HttpGet("getSummarySales/{dateFrom?}/{dateTo?}/{branchId?}/{userId?}/{priceListId?}")]
        public IActionResult GetSummarySales(DateTime dateFrom, DateTime dateTo, int branchId = 0, int userId = 0, int priceListId = 0)
        {
            var summarySales = new List<DevSummarySale>();
            var timeFrom = dateFrom.TimeOfDay;
            var timeTo = dateTo.TimeOfDay;
            summarySales = _reportSaleRepo.GetSummarySalesPos(dateFrom, dateTo, timeFrom, timeTo, branchId, userId, priceListId);
            return Ok(summarySales);
        }

    }
}
