using System.Collections.Generic;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;

namespace KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView
{
    public class GeneralSettingAdminViewModel
    {
        public Display Display { get; set; }
        public AuthorizationTemplate AuthTemplate { set; get; }
        public CardMemberTemplate CardMemberTemplate { set; get; }
        public ColorSetting Color { get; set; }
        public FontSetting Font { get; set; }
        public List<ColorSetting> ColorSetting { get; set; }
        public bool SkinUser { get; set; }

    }
}
