using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ServiceContractTemplate
{
    [Table("ContractTemplate")]
    public class ContractTemplate
    {
        [Key]
        public int ID { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string SerchName { get; set; }
        [NotMapped]
        public int RemarksID { get; set; }

        public int ConverageID { get; set; }
        public string ContractName { get; set; }
        public int ContracType { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string ContracOfType { get; set; }
        public bool Expired { get; set; }
        public string ResponseTime { get; set; }
        public int ResponseTimeDH { get; set; }
        public string ResultionTime { get; set; }
        public int ResultionTimeDH { get; set; }
        //public bool Duration { get; set; }
        //public bool Renewal { get; set; }
        //public string Reminder { get; set; }



        public Remark Remark { get; set; }
        public Converage Converage { get; set; }
        public List<AttachmentFileOfContractTemplate> AttachmentFiles { get; set; }

        [NotMapped]
        public string Remarks { get; set; }
        [NotMapped]
        public bool Part { get; set; }
        [NotMapped]
        public bool Labor { get; set; }
        [NotMapped]
        public bool Travel { get; set; }
        [NotMapped]
        public bool Holiday { get; set; }
        [NotMapped]
        public bool Monthday { get; set; }
        [NotMapped]
        public bool Thuesday { get; set; }
        [NotMapped]
        public bool Wednesday { get; set; }
        [NotMapped]
        public bool Thursday { get; set; }
        [NotMapped]
        public bool Friday { get; set; }
        [NotMapped]
        public bool Saturday { get; set; }
        [NotMapped]
        public bool Sunday { get; set; }
        [NotMapped]
        public string StarttimeMon { get; set; }
        [NotMapped]
        public string StarttimeThu { get; set; }
        [NotMapped]
        public string StarttimeWed { get; set; }
        [NotMapped]
        public string StarttimeThur { get; set; }
        [NotMapped]
        public string StarttimeFri { get; set; }
        [NotMapped]
        public string StarttimeSat { get; set; }
        [NotMapped]
        public string StarttimeSun { get; set; }
        [NotMapped]
        public string EndtimeMon { get; set; }
        [NotMapped]
        public string EndtimeThu { get; set; }
        [NotMapped]
        public string EndtimeWed { get; set; }
        [NotMapped]
        public string EndtimeThur { get; set; }
        [NotMapped]
        public string EndtimeFri { get; set; }
        [NotMapped]
        public string EndtimeSat { get; set; }
        [NotMapped]
        public string EndtimeSun { get; set; }


    }
    public enum ResOrResoType
    {
        [Display(Name = "Day(s)")]
        Day = 1,
        [Display(Name = "Hour(s)")]
        Hour = 2
    }
    public enum ContractType
    {
        [Display(Name = "")]
        None = 0,
        Customer = 1,
        [Display(Name = "Item Group")]
        ItemGroup = 2,
        [Display(Name = "Serial Number")]
        SerialNumber = 3
    }
    public enum ReminderType
    {
        [Display(Name = "")]
        None = 0,
        [Display(Name = "Day(s)")]
        Day = 1,
        [Display(Name = "Week(s)")]
        Week = 2,
        [Display(Name = "Month(s)")]
        Month = 3
    }
    public enum Times
    {
        Hours = 1,
        Days = 2,

    }
}
