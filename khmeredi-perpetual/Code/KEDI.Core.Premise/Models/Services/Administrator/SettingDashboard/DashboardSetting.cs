using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKBS.Models.Services.Administrator.SettingDashboard
{
    public class DashboardSetting
    {
        [Key]
        public int ID { get; set; }
        //public bool UserID { get; set; }
        public string Code { get; set; }
        public int CompanyID { get; set; }
        public bool Show { get; set; }
    }

    public class DashboardModel
    {
        public List<DashboardSetting> DashboardSetting { get; set; }
        public Display GeneralSetting { get; set; }
    }
}
