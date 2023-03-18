using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Activity
{
    [Table("Activity")]
    public class Activity
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int EmpID { get; set; }
        public int AssignByID { get; set; }

        public Activities Activities { get; set; }
        public int AssignByName { get; set; }
        public AssignedTo EmpNameID { get; set; }
        public int SubNameID { get; set; }
        [NotMapped]
        public string SubName { get; set; }
        [NotMapped]
        public string Employee { get; set; }
        public int TypeID { get; set; }
        [NotMapped]
        public string TypeName { get; set; }
        [NotMapped]
        public string ContactPerson { get; set; }
        public int Number { get; set; }
        public int BPID { get; set; }
        [NotMapped]
        public string TelNo { get; set; }
        public bool Personal { get; set; }
        public General General { get; set; }

        //===============================//
        [NotMapped]
        public int Act { get; set; }
        [NotMapped]
        public string ContactID { get; set; }
        [NotMapped]
        public List<ActivityViewData> ActivityViewDatas { get; set; }
        [NotMapped]
        public int GID { get; set; }
        [NotMapped]

        public int SubID { get; set; }
        [NotMapped]

        public int AssignedID { get; set; }
        [NotMapped]

        public int ActivityID { get; set; }
        [NotMapped]

        public string SetActName { get; set; }
        [NotMapped]

        public string BpCode { get; set; }
        [NotMapped]


        public string BpType { get; set; }
        [NotMapped]

        public string BpName { get; set; }
        [NotMapped]

        //=====generl===
        public string Remark { get; set; }
        [NotMapped]

        public DateTime StartTime { get; set; }
        [NotMapped]

        public DateTime EndTime { get; set; }
        [NotMapped]

        public string Durration { get; set; }
        [NotMapped]

        public string Status { get; set; }
        [NotMapped]

        public string Recurrences { get; set; }
        [NotMapped]

        public string Priority { get; set; }
        [NotMapped]

        public string Location { get; set; }
        [NotMapped]

        public string StartClock { get; set; }
        [NotMapped]

        public string EndClock { get; set; }

        //===daily===
        [NotMapped]

        public int RepeatDate { get; set; }
        [NotMapped]

        public bool RepeatEveryRecurr { get; set; }
        [NotMapped]

        public bool RepeatEveryWeek { get; set; }
        //==Enddaily====
        //===Weekly==
        [NotMapped]

        public bool Mon { get; set; }
        [NotMapped]

        public bool Tue { get; set; }
        [NotMapped]

        public bool Wed { get; set; }
        [NotMapped]

        public bool Thu { get; set; }
        [NotMapped]

        public bool Fri { get; set; }
        [NotMapped]

        public bool Sat { get; set; }
        [NotMapped]

        public bool Sun { get; set; }

        //====MONTHLY===
        [NotMapped]

        public int RepeatNumOfmonths { get; set; }
        [NotMapped]

        public bool Days { get; set; }
        [NotMapped]

        public int numDay { get; set; }
        [NotMapped]

        public bool repeatOn { get; set; }
        [NotMapped]

        public string numOfRepeat { get; set; }
        [NotMapped]

        public string DaysInMonthly { get; set; }
        [NotMapped]

        //========yealry======
        public int RepeatofNumAnnualy { get; set; }
        [NotMapped]

        public bool RepeatOncheckYearly { get; set; }
        [NotMapped]

        public string MonthsInAnnualy { get; set; }
        [NotMapped]

        public int NumOfMonths { get; set; }
        [NotMapped]

        public bool checkNumAnnualy { get; set; }
        [NotMapped]

        public string NumofAnnualy { get; set; }
        [NotMapped]

        public string DaysOfAnnualy { get; set; }
        [NotMapped]

        public string MonthsOfAnnulay { get; set; }
        [NotMapped]

        public DateTime Start { get; set; }
        [NotMapped]

        public bool NoEndDate { get; set; }
        [NotMapped]

        public bool After { get; set; }
        [NotMapped]

        public int NumAfter { get; set; }
        [NotMapped]

        public bool By { get; set; }
        [NotMapped]

        public DateTime ByDate { get; set; }
        [NotMapped]

        public List<SelectListItem> AssignedTo { get; set; }
        [NotMapped]

        public List<SelectListItem> Priorities { get; set; }
        [NotMapped]

        public List<SelectListItem> Recurrence { get; set; }
        [NotMapped]
        public List<ContactPerson> Contact { get; set; }
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        [NotMapped]
        public string EmpName { get; set; }


    }

    public enum AssignedTo
    {
        //User = 1,
        Employee = 2,
        //RecipientList = 3,

    }
    public enum Recurrence
    {
        None = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4,
        Annually = 5

    }
    public enum Priorities
    {
        None = 0,
        Low = 1,
        Normal = 2,
        Hight = 3,


    }
    public enum Activities
    {
        Sales = 1,
        Services = 2,
        Purchases = 3,
        Stock = 4,
        Financials = 5,
        Productions = 6,
        Marketing = 7,
        HRandAdmin = 8,
        General = 9,
        ResearchAndDevelopment = 10,
        Logistics = 11,
        IT = 12,
        MechanicalEngineering = 13,
        Investment = 14,
        Construction = 15,
        QualityAssurance = 16,
        InternalAudit = 17


    }
}
