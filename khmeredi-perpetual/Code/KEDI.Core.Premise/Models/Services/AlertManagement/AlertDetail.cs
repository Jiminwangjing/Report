using System.Collections.Generic;

namespace CKBS.Models.Services.AlertManagement
{
    public class AlertDetail
    {
        public int ID { get; set; }
        public int AlertMasterID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public List<AlertWarehouses> AlertWarehouses { get; set; }
        public List<UserAlert> UserAlerts { get; set; }
        public int CompanyID { get; set; }
        public bool IsAllWh { get; set; }
        public bool IsAllUsers { get; set; }
    }
}
