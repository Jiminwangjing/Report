using CKBS.AppContext;
using CKBS.Controllers.API.QRCodeV1.ClassView;
using CKBS.Controllers.API.QRCodeV1.Security;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Controllers.API.QRCodeV1
{
    [Route("/api/v1/{controller}/{action}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QRCodeAPIController : Controller
    {
        private readonly DataContext _context;
        private readonly IDataProtector _protector;
        private readonly IPOSOrderQRCodeRepository _posQr;

        public QRCodeAPIController(DataContext context, IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings, IPOSOrderQRCodeRepository posQR)
        {
            _context = context;
            _protector = dataProtectionProvider.CreateProtector(
               dataProtectionPurposeStrings.TableIDRouteValue);
            _posQr = posQR;
        }
        [HttpPost]
        public IActionResult GenerateQRCode([FromForm] QRcodeViewModel qRcode)
        {
            //http://192.168.0.181:8899/POS/KRMS
            if (qRcode.EncryptedTableID != "")
            {
                qRcode.QrCodeString = $"{Request.Scheme}://{Request.Host.Value}/POSQRCode/Index/{qRcode.EncryptedTableID}";
                var data = _posQr.GenerateQRCode(qRcode);
                return Ok(data);
            }
            return Ok(new { Error = true });
        }

        [HttpPost]
        public IActionResult Send([FromForm] string data, string printType)
        {
            Order order = JsonConvert.DeserializeObject<Order>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Order _order = _context.Order.AsNoTracking().FirstOrDefault(i=> i.OrderID == order.OrderID);
            if (order.OrderID > 0 && _order == null)
            {
                return Ok(new { Error=true, Message="Order has been paid."});
            }
            var returnItems = _posQr.Send(order, printType);
            return Ok(returnItems);
        }

        [HttpGet("{code}")]
        public IActionResult CheckPrivilege(string code)
        {
            ModelMessage message = new ();
            var privilege = GetUserPrivileges().FirstOrDefault(p => string.Compare(p.Code, code) == 0);
            if (privilege == null || !privilege.Used)
            {
                message.Add(code, "You have no privilege to access this feature.");
            }
            else
            {
                message.Add(code, "Access granted.");
                message.Approve();
            }
            return Ok(new { Privilege = privilege, Message = message });
        }
        [HttpGet("{group1}/{group2}/{group3}/{priceListId}/{level}/{onlyAddon}")]
        public async Task<IActionResult> GetGroupItems(int group1, int group2, int group3, int priceListId, int level = 0, bool onlyAddon = false)
        {
            var itemInfos = await _posQr.GetGroupItemsAsync(group1, group2, group3, priceListId, level, onlyAddon);
            return Ok(itemInfos);
        }
        [HttpGet("{tableId}/{orderId}/{setDefaultOrder}")]
        public async Task<IActionResult> FetchOrderInfoQR(string tableId, int orderId = 0, bool setDefaultOrder = false)
        {
            try
            {
                string decryptedId = tableId;//_protector.Unprotect(tableId);
                int decryptedIntId = Convert.ToInt32(decryptedId);
                var orderInfo = await _posQr.FetchOrdersQR(decryptedIntId, orderId, setDefaultOrder);
                return Ok(orderInfo);
            }
            catch
            {
                return Ok(new { Error = true, Message = "Invalid url please try agian!"});
            }
            
        }
        IEnumerable<UserPrivillege> GetUserPrivileges(int userId = 0)
        {
            if (userId == 0)
            {
                userId = GetUserID().ID;
            }
            return _context.UserPrivilleges.Where(w => w.UserID == userId && w.Function.Type == "POS" && !w.Delete).ToList();
        }
        private UserAccount GetUserID()
        {
            var user = _context.UserAccounts.FirstOrDefault(i => i.IsUserOrder) ?? new UserAccount();
            return user;
        }
    }
}
