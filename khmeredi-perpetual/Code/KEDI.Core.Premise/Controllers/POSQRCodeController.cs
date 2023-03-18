using CKBS.Controllers.API.QRCodeV1.Security;
using CKBS.Models.SignalR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CKBS.Controllers
{
    [Route("[Controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class POSQRCodeController : Controller
    {
        [HttpGet("Index/{tableId?}")]
        public IActionResult Index(string tableId = "")
        {
            return View();
        }
    }
}
