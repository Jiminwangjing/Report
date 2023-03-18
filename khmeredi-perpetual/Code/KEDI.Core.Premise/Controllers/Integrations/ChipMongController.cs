using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Integrations.Aeon;
using KEDI.Core.Premise.Repositories.Integrations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers.Integrations
{
    [Privilege]
    public class ChipMongController : Controller
    {
        private readonly IChipMongRepo _chipMongRepo;
        public ChipMongController(IChipMongRepo chipMongRepo)
        {
            _chipMongRepo = chipMongRepo;
        }

        public IActionResult ReportSales()
        {
            ViewBag.ChipMongReportSales = "highlight";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetReportSales(string dateFrom, string dateTo)
        {
            _ = DateTime.TryParse(dateFrom, out DateTime _dateFrom);
            _ = DateTime.TryParse(dateTo, out DateTime _dateTo);
            var tenantSales = await _chipMongRepo.GetReportSalesAsync(_dateFrom, _dateTo);
            return Ok(tenantSales);
        }
    }
}
