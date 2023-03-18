using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Integrations;
using KEDI.Core.Premise.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Controllers
{
    public class DataSyncSettingController : Controller
    {
        private readonly ILogger<DataSyncSettingController> _logger;

        private readonly IDataSyncSettingRepo _dataSyncRepo;
        public DataSyncSettingController(ILogger<DataSyncSettingController> logger, 
            IDataSyncSettingRepo dataSyncSetting)
        {
            _logger = logger;
            _dataSyncRepo = dataSyncSetting;
        }

        public async Task<IActionResult> Index()
        {  
            var dataSyncSettins = await _dataSyncRepo.GetDataSyncSettingsAsync();
            return View(dataSyncSettins);
        }

        public async Task<IActionResult> Submit(int id = 0){
            var setting = await _dataSyncRepo.FindAsync(id);
            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(DataSyncSetting dataSyncSetting){         
            if(ModelState.IsValid){    
                await _dataSyncRepo.SaveAsync(ModelState, dataSyncSetting);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult>Revoked(int id) {
            await _dataSyncRepo.RevokedAsync(id);
            return RedirectToAction(nameof(Index));
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}