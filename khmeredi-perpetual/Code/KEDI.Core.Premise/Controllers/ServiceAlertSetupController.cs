using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KEDI.Core.Premise.Controllers
{
    public class ServiceAlertSetupController : Controller
    {
        private readonly DataContext _context;

        public ServiceAlertSetupController(DataContext context)
        {
            _context = context;
        }
        public IActionResult ServiceAlert()
        {
            ViewBag.ServiceAlert = "highlight";
            return View();
        }
        public IActionResult GetService()
        {
            var data = _context.GeneralServiceSetups.ToList();
            return Ok(data);
        }
        public IActionResult UpdateService(GeneralServiceSetup generalService)
        {
            _context.GeneralServiceSetups.Update(generalService);
            _context.SaveChanges();

            return Ok();
        }
    }
}
