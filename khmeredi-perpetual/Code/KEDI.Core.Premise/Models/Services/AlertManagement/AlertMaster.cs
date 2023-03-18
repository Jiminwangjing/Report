using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.AlertManagement
{
    public class AlertMaster
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public TypeOfAlert TypeOfAlert { get; set; }
        public bool Active { get; set; } 
        public DeleteAlert DeleteAlert { get; set; }
        public long Frequently { get; set; }
        public TypeFrequently TypeFrequently { get; set; }
        public TypeFrequently TypeBeforeAppDate { get; set; }
        public long BeforeAppDate { get; set; }
        public int CompanyID { get; set; }
    }
    public enum DeleteAlert { False, True }
    public enum StatusAlert { Inactive, Active }
    public enum StatusAlertUser { Inactive, Active }
    public enum TypeFrequently { Select, Minutes, Hours, Days, Weeks, Months, Years }
    public enum TypeOfAlert
    {
        [Display(Name = "")]
        Null,
        [Display(Name = "Stock")]
        Stock,
        [Display(Name = "Expire Item")]
        ExpireItem,
        [Display(Name = "Appointment")]
        Appointment,
        [Display(Name = "Due Date")]
        DueDate,
        [Display(Name = "Cash Out")]
        CashOut,
        [Display(Name = "Payment")]
        Payment,
        [Display(Name = "AR Service Contract")]
        ARServiceContract,
    }
}
