using CKBS.AppContext;
using KEDI.Core.Hosting;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Security;
using KEDI.Core.System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class SystemController : Controller
    {
        private readonly HostMessenger _messenger;
        private readonly SystemManager _systemModule;
        public SystemController(SystemManager systemModule, UserManager userModule, DataContext context, HostMessenger messenger)
        {
            _messenger = messenger;
            _systemModule = systemModule;
        }

        [AllowAnonymous]
        public IActionResult Activate()
        {
            if (_systemModule.IsActivated())
            {
                return Redirect("/account/redirectUser");
            }
            
            if(User.Identity.IsAuthenticated){ return Redirect("/account/logout"); }
            return View(new SystemLicense());
        }

        public IActionResult Reactivate()
        {
            return View(nameof(Activate), new SystemLicense());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ImportLicense()
        {
            ModelMessage message = new();
            SystemLicense license = new();
            if (Request.Form.Files.Count > 0)
            {
                IFormFile formFile = Request.Form.Files[0];
                if (formFile.Length > 0)
                {
                    license = await _systemModule.ActivateAsync(formFile.OpenReadStream(), ModelState);
                    if (_systemModule.VerifyLicense(license))
                    {
                        ModelState.AddModelError("License", "The license is verified.");
                        message.Approve();
                    }
                    else
                    {
                        ModelState.AddModelError("License", "The license is invalid.");
                    }
                }
                message.AddItem(license, "License");
            }

            return Ok(message.Bind(ModelState));
        }
    }
}
