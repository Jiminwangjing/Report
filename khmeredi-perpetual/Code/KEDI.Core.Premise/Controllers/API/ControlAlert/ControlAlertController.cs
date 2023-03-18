using CKBS.Models.Services.Responsitory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Controllers.API.V1.ControlAlert
{
    [Route("/api/{controller}/{action}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ControlAlertController : Controller
    {
        private readonly IControlAlertAPIRepository _alert;

        public ControlAlertController(IControlAlertAPIRepository controlAlert)
        {
            _alert = controlAlert;
        }
        [HttpGet("{code}")]
        public async Task<IActionResult> GetStockAlertByCode(string code)
        {
            var data = await _alert.GetStockAlertByCodeAsync(code);
            return Ok(data);
        }
        [HttpGet("{active}/{inactive}")]
        public async Task<IActionResult> GetItems(bool active, bool inactive)
        {
            var data = await _alert.GetItemsAsync(active, inactive);
            return Ok(data);
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> GetStockAlertByName(string name)
        {
            var data = await _alert.GetStockAlertByNameAsync(name);
            return Ok(data);
        }
    }
}
