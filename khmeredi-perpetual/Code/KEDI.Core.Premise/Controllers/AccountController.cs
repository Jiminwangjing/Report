using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Responsitory;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.Services.Account;
using System.Drawing;
using QRCoder;
using Microsoft.AspNetCore.SignalR;
using KEDI.Core.Premise.Models.SignalR;
using KEDI.Core.Models.Validation;

namespace CKBS.Controllers
{
    [Privilege]
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserAccount _user;
        private readonly IEmployee _employee;
        private readonly UserManager _userModule;
        private readonly SystemManager _systemModule;
        readonly IHubContext<HubSystem, IReceiver> _hubContext;
        public AccountController(DataContext context, SystemManager systemModule,
            UserManager userModule, IUserAccount user, IEmployee employee, IHubContext<HubSystem, IReceiver> hubContext)
        {
            _userModule = userModule;
            _systemModule = systemModule;
            _context = context;
            _user = user;
            _employee = employee;
            _hubContext = hubContext;
        }

        [Privilege("A004")]
        public IActionResult Index()
        {
            ViewBag.UserAccount = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetAccessTokenQR(string username)
        {
            var token = _userModule.CreateAccessToken(username);
            QRCodeGenerator qrCodeGenerator = new();
            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            Bitmap bitmap = qrCode.GetGraphic(5);
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return File(memoryStream.ToArray(), "image/png"); //Return as file result
        }

        static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetUserAccounts(string keyword = "")
        {

            var users = _userModule.GetUsers();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                users = users.Where(c => RawWord(c.Username).Contains(keyword, ignoreCase)).ToList();
            }
            return Ok(users);
        }

        [HttpGet]
        [Privilege("A004")]
        public IActionResult Register()
        {
            ViewBag.UserAccount = "highlight";
            ViewData["BranchID"] = new SelectList(_context.Branches.Where(x => x.Delete == false), "ID", "Name");
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name");
            ViewData["EmployeeID"] = new SelectList(_context.Employees.Where(e => e.Delete == false && e.IsUser == false), "ID", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Privilege("A004")]
        public async Task<IActionResult> Register(UserAccount userAccount)
        {
            ViewBag.UserAccount = "highlight";
            await _userModule.CreateUserAsync(userAccount, ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchID"] = new SelectList(_context.Branches.Where(x => !x.Delete), "ID", "Name", userAccount.BranchID);
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => !c.Delete), "ID", "Name", userAccount.CompanyID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees.Where(e => !e.Delete && !e.IsUser), "ID", "Name", userAccount.EmployeeID);
            return View(userAccount);
        }

        [HttpGet]
        [Privilege("A004")]
        public IActionResult Edit(int id)
        {
            ViewBag.UserAccount = "highlight";
            var user = _userModule.FindById(id);
            if (user.Delete)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchID"] = new SelectList(_context.Branches.Where(x => !x.Delete), "ID", "Name");
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => !c.Delete), "ID", "Name");
            ViewData["EmployeeID"] = new SelectList(_context.Employees.Where(e => !e.Delete), "ID", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserAccount userAccount)
        {
            ViewBag.UserAccount = "highlight";
            await _userModule.EditUserAsync(userAccount, ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchID"] = new SelectList(_context.Branches.Where(x => x.Delete == false), "ID", "Name", userAccount.BranchID);
            ViewData["CompanyID"] = new SelectList(_context.Company.Where(c => c.Delete == false), "ID", "Name", userAccount.CompanyID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees.Where(e => e.Delete == false && e.IsUser == false), "ID", "Name", userAccount.EmployeeID);
            return View(userAccount);
        }

        public string Encrypt(string clearText)
        {
            if (clearText != null)
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
                return clearText;
            }
            else
            {
                return clearText;
            }

        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
            return cipherText;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (!_userModule.IsActivated()){ return Redirect("/system/activate"); }
            //var checkedUser = _context.UserAccounts.FirstOrDefault() ?? new UserAccount();
            //if (string.IsNullOrWhiteSpace(checkedUser.PasswordStamp))
            //{
            //    var users = _context.UserAccounts.ToList();
            //    foreach (var user in users)
            //    {
            //        _userModule.HashPassword(user, Decrypt(user.Password));
            //        _context.SaveChanges();
            //    }
            //}

            var loginModel = new LoginViewModel
            {
                Logo = _context.Company.Select(c => c.Logo).FirstOrDefault()
            };
            return View(loginModel);
        }

        public IActionResult RedirectUser()
        {
            var user = _userModule.CurrentUser;
            if (user.UserPos)
            {
                if (_userModule.SystemTypes["KRMS"]) { return RedirectToAction("KRMS", "POS"); }
                if (_userModule.SystemTypes["KTMS"]) { return RedirectToAction("KTMS", "POS"); }
                if (_userModule.SystemTypes["KBMS"]) { return RedirectToAction("KBMS", "POS"); }
                if (_userModule.SystemTypes["KSMS"]) { return RedirectToAction("KSMS", "POS"); }
            }
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginView/*, string returnUrl*/)
        {
            if (ModelState.IsValid)
            {
                bool passwordVerified = _userModule.VerifyPassword(loginView.Username, loginView.Password);
                if (!passwordVerified)
                {
                    ModelState.AddModelError("Password", "Username or password is incorrect.");
                }
                else
                {
                    var user = await _userModule.FindUserAsync(loginView.Username);
                    if (!User.Identity.IsAuthenticated)
                    {
                        if (_userModule.ReachedMaxUsers())
                        {
                            ModelState.AddModelError("Password", "Max users reached limit.");
                            return View(loginView);
                        }
                    }

                    List<Claim> lists = new()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                        new Claim("ID", user.ID.ToString()),
                        new Claim("Code", user.Employee.Code),
                        new Claim(ClaimTypes.Name, user.ID.ToString()),
                        new Claim("Username", user.Username),
                        new Claim("UserID", user.ID.ToString()),
                        new Claim("Photo", (user.Employee.Photo != null) ? "~/Images/employee/" + user.Employee.Photo : "~/Images/User/default_user.png"),
                        new Claim("FullName", user.Employee.Name ?? ""),
                        new Claim("Position", user.Employee.Position ?? ""),
                        new Claim("Phone", user.Employee.Phone.ToString() ?? ""),
                        new Claim("Email", user.Employee.Email ?? ""),
                        new Claim("Logo", (user.Company.Logo != null) ? "~/Images/company/" + user.Company.Logo : "~/Images/User/Logo.gif"),
                        new Claim("CompanyName", user.Company.Name ?? ""),
                        new Claim("CompanyID", user.Company.ID.ToString()),
                        new Claim("BranchID", user.BranchID.ToString())
                    };

                    var identity = new ClaimsIdentity(lists, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var props = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
                    var aiUser = HttpContext.Request.Cookies["ai_user"];
                    _userModule.AddLoggedUser($"{user.ID}", $"{user.ID}");
                    await _hubContext.Clients.User(user.ID.ToString()).SignOut();
                    return RedirectToAction(nameof(RedirectUser));
                }
            }

            loginView.Logo = _context.Company.Select(c => c.Logo).FirstOrDefault();
            return View(loginView);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var props = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow
                };
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, props);
                _userModule.RemoveLoggedUser($"{GetUserID()}");
            }
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Privilege("A043")]
        public IActionResult UserPrivileges(int userId)
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Set User Privilege";
            ViewBag.UserAccount = "highlight";
            ViewBag.General = "show";
            UserPrivilegeModel privilegeModel = new();
            privilegeModel.UserPrivilleges = _user.GetUserPrivilleges(userId).ToList();
            return View(privilegeModel);
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        [ValidateAntiForgeryToken(Order = 2)]
        public IActionResult UpdateUserPrivilege(UserPrivilegeModel privilegeModel)
        {
            if (ModelState.IsValid)
            {
                _user.UpdateUserPrivilleges(privilegeModel.UserPrivilleges.ToList());
            }

            return RedirectToAction(nameof(Index));
        }
        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int _userId);
            return _userId;
        }

        [HttpPost]
        public IActionResult SelectAll(bool All, int UserID)
        {
            _user.UpdateAllselect(All, UserID);
            return Ok();
        }

        public IActionResult ChangePassword()
        {
            ViewBag.UserAccount = "highlight";
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            ViewBag.UserAccount = "highlight";
            _userModule.ChangePassword(GetUserID(), model, ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Logout));
            }
            return View(new ChangePasswordModel());
        }

        public IActionResult Profile()
        {
            ViewBag.UserAccount = "highlight";
            var user = _userModule.GetUser(User);
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _user.Delete(id);
            var User = _context.UserAccounts.FirstOrDefault(x => x.ID == id);
            var Emp = _context.Employees.FirstOrDefault(x => x.ID == User.EmployeeID);
            Emp.IsUser = false;
            _context.Employees.Update(Emp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult SetSetting(int id)
        {
            var BranchID = _context.UserAccounts.FirstOrDefault(w => w.ID == id);
            ViewBag.SettingID = id;
            ViewBag.BranchID = BranchID.BranchID;
            return View();
        }
        [Privilege("A00001")]
         [HttpGet]
        public IActionResult GetBrand(int userId)
        {
            ViewBag.style = "fa fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "MultiBrand";
            ViewBag.UserAccount = "highlight";
            ViewBag.General = "show";
            ViewData["UserID"] = userId;
            //CheckMultiBrand(userId);
            return View();
        }
        // public void CheckMultiBrand(int userId)
        // {
        //     var Brand = _context.Branches.AsNoTracking().Where(w => !w.Delete).ToList();
        //     var checkmb = _context.MultiBrands.Where(s => s.UserID == userId).ToList();

        //     foreach (var b in Brand){

        //         if (checkmb.Count == 0)
        //         {
        //             MultiBrand multibrands = new();

        //             multibrands.ID = 0;
        //             multibrands.BranchID = b.ID;
        //             multibrands.UserID = userId;
        //             multibrands.Active = false;

        //             _context.Add(multibrands);
        //         }
        //         else{
        //             var multibrand = _context.MultiBrands.FirstOrDefault(w=>w.UserID == userId);

        //             multibrand.ID = multibrand.ID;
        //             multibrand.UserID = userId;
        //             _context.Update(multibrand);
        //         }
        //         _context.SaveChanges();
        //     }

        // }
         [HttpGet]
        private async Task<List<MultiBrand>> GetBranchs(int userId)
        {
            var objectUser =await _context.UserAccounts.FirstOrDefaultAsync(s => s.ID == userId ) ?? new UserAccount();
            var list = (from b in _context.Branches.Where(x => !x.Delete)
                        select new MultiBrand
                        {
                            ID      = 0,
                            LineID  = DateTime.Now.Ticks.ToString() + b.ID.ToString(),
                            Name    = b.Name,
                            UserID  = 0,
                            Active  =objectUser.BranchID == b.ID ? true: false,
                            BranchID = b.ID,
                            Defualt =  objectUser.BranchID==b.ID?true:false,
                       });
            return await list.ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> MultiBrand(int userId)
        {
            List<MultiBrand> branch=new();
            branch = await GetBranchs(userId);
            var _multi = await _context.MultiBrands.Where(w => w.UserID == userId).ToListAsync();

            for (int i = 0; i < branch.Count; i++)
            {
                for (int j = 0; j < _multi.Count; j++)
                {
                    if (branch[i].BranchID == _multi[j].BranchID)
                    {
                        branch[i].ID = _multi[j].ID;
                        branch[i].Active = _multi[j].Active;
                        branch[i].UserID = _multi[j].UserID;
                    }
                }
            }
            return Ok(branch);
        }
        [HttpPost]
        public IActionResult SubmitMultiBrand(List<MultiBrand> data)
        {
            ModelMessage msg = new();
             if(data.Count ==0){
                    ModelState.AddModelError("data", "Please Check Active brand less than 1 !");
                }
           
            var obj = data.FirstOrDefault(i => i.Defualt == true);
            
            if(obj==null|| obj.Defualt==false){
                    ModelState.AddModelError("", "Please Check Defualt brand.. !");
            }
              if (ModelState.IsValid)
            {
                
                var user = _context.UserAccounts.FirstOrDefault(w => w.ID ==obj.UserID);
                if(user != null){
                    user.BranchID = obj.BranchID;
                    _context.UserAccounts.UpdateRange(user);
                }
                _context.MultiBrands.UpdateRange(data);
                _context.SaveChanges();     
                ModelState.AddModelError("success", "Save successfully.");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
    }
}