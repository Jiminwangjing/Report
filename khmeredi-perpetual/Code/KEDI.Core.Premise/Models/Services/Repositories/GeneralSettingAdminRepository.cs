using CKBS.AppContext;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface IGeneralSettingAdminRepository
    {
        Task<Display> GetDisplayAsync();
        Task CreateDisplayAsync(Display display);
        int SaveAuthTemplate(AuthorizationTemplate templateOption, ModelStateDictionary modelState);
        int SaveCardMemberTemplate(CardMemberTemplate template, ModelStateDictionary modelState);
        AuthorizationTemplate GetAuthTemplate();
        CardMemberTemplate GetCardMemberTemplate();
        List<ColorSetting> GetSkinthem();
    }
    public class GeneralSettingAdminRepository : IGeneralSettingAdminRepository
    {
        private readonly DataContext _context;
        private readonly UserManager _userModule;
        ILogger<GeneralSettingAdminRepository> _logger;
        public GeneralSettingAdminRepository(ILogger<GeneralSettingAdminRepository> logger, DataContext context, UserManager userModule)
        {
            _logger = logger;
            _context = context;
            _userModule = userModule;
        }
        public async Task CreateDisplayAsync(Display display)
        {
            _context.Displays.Update(display);
            await _context.SaveChangesAsync();
        }

        public async Task<Display> GetDisplayAsync()
        {
            var display = await _context.Displays.FirstOrDefaultAsync() ?? new Display();
            return display;
        }
        public AuthorizationTemplate GetAuthTemplate()
        {
            return _context.AuthorizationTemplates.FirstOrDefault() ?? new AuthorizationTemplate();
        }
        public CardMemberTemplate GetCardMemberTemplate()
        {
            return _context.CardMemberTemplates.FirstOrDefault() ?? new CardMemberTemplate();
        }
        public int SaveAuthTemplate(AuthorizationTemplate template, ModelStateDictionary modelState)
        {
            try
            {
                if (modelState.IsValid)
                {
                    _context.AuthorizationTemplates.Update(template);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return -1;
        }
        public int SaveCardMemberTemplate(CardMemberTemplate template, ModelStateDictionary modelState)
        {
            try
            {
                if (modelState.IsValid)
                {
                    _context.CardMemberTemplates.Update(template);
                    return _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return -1;
        }
        public List<ColorSetting> GetSkinthem()
        {
            var user = _userModule.CurrentUser;
            var list = (from _data in _context.ColorSettings
                        join SkinUser in _context.SkinUser.Where(w => w.UserID == user.ID) on _data.ID equals SkinUser.SkinID
                        select new ColorSetting
                        {
                            ID = _data.ID,
                            SkinName = _data.SkinName,
                            BackgroundColorName = _data.BackgroundColorName,
                            HoverColor = _data.HoverColor,
                            designName = _data.designName,
                            checkClick = _data.checkClick,
                            AlphabetColor = _data.AlphabetColor,
                            Unable = SkinUser.Unable,
                        }).ToList();
            return list;
        }
    }
}
