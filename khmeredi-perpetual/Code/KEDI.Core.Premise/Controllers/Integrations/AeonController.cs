using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Integrations.Aeon;
using KEDI.Core.Premise.Repositories.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Controllers.Integrations
{
    [Privilege]
    public class AeonController : Controller
    {
        private readonly IAeonRepo _aeonRepo;

        public AeonController(IAeonRepo aeonRepo)
        {
            _aeonRepo = aeonRepo;
        }

        public IActionResult ReportSales()
        {
            ViewBag.AeonReportSales = "highlight";
            var saleTypes = Enum.GetValues<TenantSaleType>()
                            .ToDictionary(k => k, v => v)
                            .Select(d => new SelectListItem{
                                Value = d.Key.ToString(),
                                Text = d.Value.ToString()
                            });
           
            ViewBag.DocumentTypes = saleTypes;              
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetReportSales(TenantSaleType saleType, string dateFrom, string dateTo)
        {
            _ = DateTime.TryParse(dateFrom, out DateTime _dateFrom);
            _ = DateTime.TryParse(dateTo, out DateTime _dateTo);
            var tenantSales = await _aeonRepo.GetReportTenantSalesAsync(saleType, _dateFrom, _dateTo);
            return Ok(tenantSales);
        }
    }
}