using ABA.Payway;
using ABA.Payway.Models;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.SignalR;
using CKBS.AppContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SecondScreenPos.Model.Payway;
using System.Collections.Generic;
using System.Linq;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using KEDI.Core.Premise.Repository;

namespace CKBS.Controllers.API
{
    [Route("/api/{controller}/{action}")]
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class POSController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IHubContext<SignalRClient> _hubContext;
        private readonly PosRetailModule _posModule;
        public POSController(DataContext dataContext, PosRetailModule posModule, IHubContext<SignalRClient> hubContext)
        {
            _dataContext = dataContext;
            _hubContext = hubContext;
            _posModule = posModule;
        }  

        private GeneralSetting GeneralSetting => _dataContext.GeneralSettings
        .FirstOrDefault(g => g.UserID == GetUserID()) ?? new GeneralSetting();

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);
            if (_dataContext.UserAccounts.Any(u => u.ID == userId))
            {
                return userId;
            }
            return 0;
        }

        [AllowAnonymous]
        public IActionResult GetCompanyInfo()
        {
            var companies = _dataContext.UserAccounts.Where(u => u.ID == 1)
                .Join(_dataContext.Company, u => u.CompanyID, c => c.ID, (u, c) => c);
            return Ok(companies.FirstOrDefault()?? new Company());
        }

        [HttpPost]
        public IActionResult SendToSecondScreen(string order, string displayCurrency)
        {
            List<DisplayPayCurrencyModel> currencies = JsonConvert.DeserializeObject<List<DisplayPayCurrencyModel>>(displayCurrency);
            Order _order = JsonConvert.DeserializeObject<Order>(order, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            if (GeneralSetting.DaulScreen)
            {
                _order.OrderDetail ??= new List<OrderDetail>();
                var invoice = _posModule.ToInvoiceDisplay(_order, currencies);
                _hubContext.Clients.All.SendAsync("ReceiveOrder", invoice);
                return Ok(new { Order = _order, Currency = currencies });
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult OnPaymentSuccess(string order)
        {
            Order _order = JsonConvert.DeserializeObject<Order>(order, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (GeneralSetting.DaulScreen)
            {
                _order.OrderDetail ??= new List<OrderDetail>();
                _hubContext.Clients.All.SendAsync("PaymentSuccess", "Payment is successful.");               
            }
            return Ok();
        }

        public IActionResult EnableDualScreen(bool enabled)
        {
            _hubContext.Clients.All.SendAsync("EnableDualScreen", GeneralSetting);
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
