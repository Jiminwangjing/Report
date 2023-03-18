using CKBS.AppContext;
using CKBS.Models.ServicesClass.DashboardSettingView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Authorize]
    public class SettingDashboardController : Controller
    {
        private readonly DataContext _context;
        public SettingDashboardController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult DashboardPrivileges()
        {
            ViewBag.SettingDashboard = "highlight";          
            var data = _context.DashboardSettings.ToList();
            DashBoardSettingViewModel dashBoardSettingViewModel = new DashBoardSettingViewModel
            {
                DashboardSettings = data,
            };
            return View(dashBoardSettingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDashboardPrivilege(DashBoardSettingViewModel dashBoardSettingViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.DashboardSettings.UpdateRange(dashBoardSettingViewModel.DashboardSettings);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(DashboardPrivileges));
        }
    }
}
