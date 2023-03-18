using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.ClientApi.Sale;
using KEDI.Core.Premise.Repositories.Integrations;
using KEDI.Core.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KEDI.Core.Premise.ApiControllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiAuthorize]
    public class TenantSaleController : ControllerBase
    {
        private readonly ITenantSaleRepo _tenantSaleRepo;
        public TenantSaleController(ITenantSaleRepo tenantSaleRepo)
        {
            _tenantSaleRepo = tenantSaleRepo;
        }

        [HttpPost("submitSaleReceipt")]
        public async Task<IActionResult> SubmitSaleReceipt([FromBody] SaleReceipt invoice)
        {
            var receipt = await _tenantSaleRepo.CreateReceiptAsync(invoice);
            return Ok(receipt);
        }
    }
}