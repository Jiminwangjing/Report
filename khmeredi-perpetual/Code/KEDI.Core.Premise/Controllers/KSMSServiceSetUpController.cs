using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.KSMS;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class KSMSServiceSetUpController : Controller
    {
        private readonly DataContext _context;
        private readonly IKSServiceSetUpRepository _ksSevice;

        public KSMSServiceSetUpController(DataContext context, IKSServiceSetUpRepository ksSevice)
        {
            _context = context;
            _ksSevice = ksSevice;
        }
        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;
        }
        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }
        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        [Route("/ksmsservicesetup/setup")]
        public IActionResult KSMSServiceSetUp()
        {
            ViewBag.KSMSServiceSetUp = "highlight";
            ViewBag.Warehouse = new SelectList(_context.Warehouses.Where(i => !i.Delete), "ID", "Name");
            ViewBag.PriceList = new SelectList(_context.PriceLists.Where(i => !i.Delete), "ID", "Name");
            return View();
        }
        [Route("/ksmsservicesetup/history")]
        public IActionResult KSMSServiceSetUpHIstory()
        {
            ViewBag.KSMSServiceSetUp = "highlight";
            return View();
        }
        public async Task<IActionResult> Getitems()
        {
            var data = await _ksSevice.GetItemsAsync();
            return Ok(data);
        }
        public async Task<IActionResult> GetitemDetials(int itemId, int plId)
        {
            var data = await _ksSevice.GetItemDetialsAsync(itemId, plId, GetCompany());
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateData(ServiceSetup serviceSetup)
        {
            ModelMessage msg = new();
            serviceSetup.UserID = GetUserID();
            await _ksSevice.UpdateDataAsync(serviceSetup, ModelState, msg);
            return Ok(msg.Bind(ModelState));
        }

        public async Task<IActionResult> GetService(string setupCode)
        {
            var data = await _ksSevice.GetServiceAsync(setupCode);
            if (data.ServiceSetUpView.ID <= 0)
            {
                return Ok(new { Error = true, Message = "Service not found!" });
            }
            return Ok(data);
        }
        public async Task<IActionResult> GetServiceHistory()
        {
            var data = await _ksSevice.GetAllServicesAsync();
            return Ok(data);
        }
    }
}
