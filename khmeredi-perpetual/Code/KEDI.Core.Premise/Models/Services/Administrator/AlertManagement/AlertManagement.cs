using CKBS.Models.Services.AlertManagement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Administrator.AlertManagement
{
    [Table("tbAlertManagement", Schema = "dbo")]
    public class AlertManagement
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Please input code !")]
        public string Code { get; set; }
        public string Name { get; set; }
        public StatusAlert StatusAlert { get; set; }
        public DeleteAlert DeleteAlert { get; set; }
        public bool ReadAlert { get; set; }
        public TypeOfAlert TypeOfAlert { get; set; }

    }

    [Table("tbSettingAlert")]
    public class SetttingAlert
    {
        [Key]
        public int ID { get; set; }
        public int AlertManagementID { get; set; }
        public int BeforeAppDate { get; set; }
        public TypeFrequently TypeBeforeAppDate { get; set; }
        public int Frequently { get; set; }
        public TypeFrequently TypeFrequently { get; set; }
        public DeleteAlert DeleteAlert { get; set; }
        public bool IsAllWh { get; set; }
        public List<SetttingAlertUser> SetttingAlertUser { get; set; }
        public List<AlertWarehouses> AlertWarehouses { get; set; } 
    }
    
    [Table("tbSettingAlertUser")]
    public class SetttingAlertUser
    {
        [Key]
        public int ID { get; set; }
        public int SetttingAlertID { get; set; }
        public int UserAccountID { get; set; }
        public string UserName { get; set; }
        public StatusAlertUser StatusAlertUser { get; set; }
    }
}
