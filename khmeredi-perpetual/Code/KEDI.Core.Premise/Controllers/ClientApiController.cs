using System;
using KEDI.Core.Models.ControlCenter.ApiManagement;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Repository;
using KEDI.Core.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KEDI.Core.Controllers
{
    [Privilege]
    public class ClientApiController : Controller
    {
        // readonly ApiManager _apiManager;
        readonly IClientApiRepo _apiKeyRepo;
        public ClientApiController(IClientApiRepo apiKeyRepo)
        {
            _apiKeyRepo = apiKeyRepo;
        }

        public IActionResult Index()
        {
            ViewBag.ApiManagement = "highlight";
          
            return View();
        }

        [HttpGet]
        public IActionResult Submit(long id = default)
        {
            ViewBag.ApiManagement = "highlight";
            var clienApi = _apiKeyRepo.FindById(id) ?? new ClientApp();
            var clientForm = new ClientForm {
                Id = clienApi.Id,
                ClientCode = clienApi.Code,
                ClientName = clienApi.Name,
                StrictIpAddress = clienApi.StrictIpAddress
            };
            return View(clientForm);
        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public IActionResult NewApiKey([FromForm] ClientForm client)
        {
            ViewBag.ApiManagement = "highlight";
            _apiKeyRepo.CreateApiKey(ModelState, client);
            var message = new ModelMessage(ModelState);
            message.AddItem(client, "Client");
            
            if (ModelState.IsValid)
            {
                message.Approve();
            }
            return Ok(message);
        }

        public IActionResult GetClientApis()
        {
            var clientApis = _apiKeyRepo.GetClientAppDisplays();
            return Ok(clientApis);
        }
    }
}
