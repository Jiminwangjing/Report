using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Controllers
{

    public class GeneralSettingAdminController : Controller
    {
        private readonly IGeneralSettingAdminRepository _generalSetting;
        private readonly DataContext _context;
        private readonly UserManager _userModule;

        public GeneralSettingAdminController(IGeneralSettingAdminRepository generalSetting, DataContext context, UserManager userModule)
        {
            _generalSetting = generalSetting;
            _context = context;
            _userModule = userModule;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.GeneralSettingAdmin = "highlight";
            var display = await _generalSetting.GetDisplayAsync();
            bool skinuserss = _context.UserAccounts.FirstOrDefault().Skin;
            var color = _generalSetting.GetSkinthem().FirstOrDefault(s => s.Unable)
                ?? _generalSetting.GetSkinthem().FirstOrDefault();
            GeneralSettingAdminViewModel generalSetting = new()
            {
                Display = display,
                AuthTemplate = _generalSetting.GetAuthTemplate(),
                CardMemberTemplate = _generalSetting.GetCardMemberTemplate(),
                ColorSetting = _generalSetting.GetSkinthem(),
                Color = color,
                SkinUser = skinuserss,
            };
            ViewBag.Skinu = skinuserss;
            return View(generalSetting);
        }

        public async Task<IActionResult> SkinItem()
        {
            ViewBag.GeneralSettingAdmin = "highlight";
            var display = await _generalSetting.GetDisplayAsync();
            GeneralSettingAdminViewModel generalSetting = new()
            {
                Display = display,
                AuthTemplate = _generalSetting.GetAuthTemplate(),
                CardMemberTemplate = _generalSetting.GetCardMemberTemplate(),

            };
            return View(generalSetting);
        }
        public IActionResult SkinUser()
        {
            ViewBag.GeneralSettingAdmin = "highlight";

            GeneralSettingAdminViewModel generalSetting = new()
            {
                AuthTemplate = _generalSetting.GetAuthTemplate(),
                CardMemberTemplate = _generalSetting.GetCardMemberTemplate(),
                ColorSetting = _generalSetting.GetSkinthem(),
            };
            return View(generalSetting);
        }

        [HttpPost]
        public IActionResult SkinItem(ColorSetting data)
        {
            ModelMessage msg = new();

            if (ModelState.IsValid)
            {
                _context.ColorSettings.Add(data);
                _context.SaveChanges();
                foreach (var user in _context.UserAccounts.Where(c => !c.Delete))
                {
                    var _userP = new SkinUser
                    {
                        SkinID = data.ID,
                        UserID = user.ID,
                    };
                    _context.SkinUser.Add(_userP);
                }
                _context.SaveChanges();
                ModelState.AddModelError("success", "Skinname created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult GetColorSetting()
        {
            var data = from s in _context.SkinUser
                       join c in _context.ColorSettings on s.SkinID equals c.ID
                       select new SkinUser
                       {
                           ID = s.ID,
                           SkinID = s.SkinID,
                           UserID = s.UserID,

                       };
            return Ok(data);
        }
        [HttpPost]
        public IActionResult PostColorSetting(SkinUser data)
        {
            List<SkinUser> skinUsers = new();
            var test = _context.SkinUser.Where(s => s.Unable == true && s.UserID == GetUserID()).ToList();

            if (test.Count == 0)
            {
                SkinUser skinUser = new();
                skinUser = _context.SkinUser.FirstOrDefault(s => s.SkinID == data.SkinID && s.UserID == GetUserID());

                skinUser.Unable = true;
                _context.Update(skinUser);
                _context.SaveChanges();
            }
            else
            {
                var allUser = _context.UserAccounts.Where(s => s.Skin == true).ToList();
                var skinUser = _context.SkinUser.Where(s => s.UserID == GetUserID()).ToList();
                foreach (var item in skinUser)
                {
                    if (data.SkinID == item.SkinID)
                    {
                        item.Unable = true;
                    }
                    else
                    {
                        item.Unable = false;
                    }

                    _context.Update(item);
                    _context.SaveChanges();
                }

                if (allUser.Count > 1)
                {
                    var allskin = _context.SkinUser.Where(s => s.SkinID == data.SkinID).ToList();
                    foreach (var items in allskin)
                    {
                        if (data.SkinID == items.SkinID)
                        {
                            items.Unable = true;
                        }
                        else
                        {
                            items.Unable = false;
                        }
                        _context.Update(items);
                        _context.SaveChanges();
                    }
                    var allskins = _context.SkinUser.Where(s => s.SkinID != data.SkinID && s.Unable == true).ToList();
                    foreach (var items in allskins)
                    {
                        items.Unable = false;

                        _context.Update(items);
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        private List<Currency> GetCurrency()
        {
            var data = _context.Currency.Where(i => !i.Delete).ToList();
            return data;
        }
        //Display
        public IActionResult GetDisplay()
        {
            var cur = GetCurrency().ToList();
            var data = (from curr in cur
                        let di = _context.Displays.FirstOrDefault(s => s.DisplayCurrencyID == curr.ID) ?? new Display()
                        select new DisplayViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ID = di.ID,
                            DisplayCurrencyID = di.DisplayCurrencyID == 0 ? curr.ID : di.DisplayCurrencyID,
                            Amounts = di.Amounts,
                            Prices = di.Prices,
                            Percent = di.Percent,
                            Rates = di.Rates,
                            Quantities = di.Quantities,
                            Units = di.Units,
                            DecimalSeparator = di.DecimalSeparator,
                            ThousandsSep = di.ThousandsSep,
                            Currency = curr.Description
                        }).ToList();
            return Ok(data);
        }
        public IActionResult CreateDisplay(string data)
        {
            List<Display> display = JsonConvert.DeserializeObject<List<Display>>(data,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            ModelMessage msg = new();
            if (display.Count <= 0)
            {
                ModelState.AddModelError("NoData", "Please input some data!");
            }
            if (ModelState.IsValid)
            {
                _context.Displays.UpdateRange(display);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Display created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult SaveAuthTemplate(GeneralSettingAdminViewModel templateOption)
        {
            ViewBag.GeneralSettingAdmin = "highlight";
            _generalSetting.SaveAuthTemplate(templateOption.AuthTemplate, ModelState);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SaveCardMemberTemplate(GeneralSettingAdminViewModel templateOption)
        {
            ViewBag.GeneralSettingAdmin = "highlight";
            _generalSetting.SaveCardMemberTemplate(templateOption.CardMemberTemplate, ModelState);
            return RedirectToAction(nameof(Index));
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }
        [HttpPost]
        public IActionResult AddFontSetting(FontSetting data)
        {
            var _data = _context.FontSettings.AsNoTracking().FirstOrDefault();
            if (_data == null)
            {
                if (data.FontName == "0" || data.FontSize == "0")
                {
                    ModelState.AddModelError("error", "Please Select Font!");
                }
            }
            ModelMessage msg = new();

            if (ModelState.IsValid)
            {
                if (_data != null)
                {
                    if (data.FontName == "0")
                    {
                        data.FontName = _data.FontName;
                    }
                    if (data.FontSize == "0")
                    {
                        data.FontSize = _data.FontSize;
                    }
                    data.ID = _data.ID;
                    _context.FontSettings.Update(data);
                }
                else
                {
                    _context.FontSettings.Add(data);
                }
                _context.SaveChanges();
                ModelState.AddModelError("success", "Color created succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult getFont()
        {
            var data = _context.FontSettings.FirstOrDefault();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult GetChecked(GeneralSettingAdminViewModel data)
        {
            List<SkinUser> skinUsers = new();
            var t = data.ColorSetting.FirstOrDefault(s => s.Unable == true);
            skinUsers = _context.SkinUser.Where(s => s.UserID == GetUserID()).ToList();

            foreach (var item in skinUsers)
            {
                if (item.SkinID == t.ID)
                {
                    item.Unable = true;
                }
                else
                {
                    item.Unable = false;
                }
                _context.Update(item);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(SkinUser));
        }
        [HttpGet]
        public IActionResult GetSS()
        {
            int UserID = 0;
            UserID = GetUserID();
            var user = _context.UserAccounts.Where(x => !x.Delete && x.Skin && x.ID == UserID).ToList();
            if (user.Count > 0)
            {
                var ps = _context.SkinUser.FirstOrDefault(x => x.Unable) ?? new SkinUser();
                if (user != null)
                {
                    var cl = _context.ColorSettings.FirstOrDefault(x => x.ID == ps.SkinID) ?? new ColorSetting();
                    return Ok(cl);
                }


            }
            else
            {
                var Ms = _context.SkinUser.FirstOrDefault(s => s.UserID == UserID && s.Unable == true);
                if (Ms != null)
                {
                    var list = _context.ColorSettings.FirstOrDefault(s => s.ID == Ms.SkinID);
                    return Ok(list);
                }
                else
                {
                    return Ok();
                };
            }
            return Ok();

        }


        [HttpPost]
        public IActionResult UpdateUserSkin(bool skin)
        {
            var _data = _context.UserAccounts.ToList();
            var skinuser = _context.SkinUser.ToList();
            foreach (var d in _data)
            {
                d.Skin = skin;
                _context.UserAccounts.Update(d);
                _context.SaveChanges();
            }
            if (skin == true)
            {
                var sk = _context.SkinUser.FirstOrDefault(x => x.Unable);
                var skk = _context.SkinUser.Where(x => x.SkinID == sk.SkinID).ToList();
                foreach (var d in skk)
                {
                    d.Unable = true;
                    _context.SkinUser.Update(d);
                    _context.SaveChanges();
                }
            }
            else
            {
                var sk = _context.SkinUser.FirstOrDefault(x => x.Unable);
                var skk = _context.SkinUser.Where(x => x.SkinID == sk.SkinID && x.UserID != GetUserID()).ToList();

                foreach (var d in skk)
                {

                    d.Unable = false;
                    _context.SkinUser.Update(d);
                    _context.SaveChanges();
                }
            }


            ModelMessage msg = new();
            // _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetSkinUserAll()
        {
            var _data = _context.UserAccounts.Where(i => !i.Delete && i.Skin == true).ToList();
            return Ok(_data);
        }

    }
}


