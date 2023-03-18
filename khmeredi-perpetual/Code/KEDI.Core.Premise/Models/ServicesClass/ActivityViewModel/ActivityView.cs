using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Activity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel
{
    public class ActivityView
    {
        public int ID { get; set; }
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int GID { get; set; }
        public int UserID { get; set; }
        public int ConID { get; set; }
        public int Number { get; set; }
        public int Activities { get; set; }

        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public DateTime StartTime { get; set; }
        public string StartTimes { get; set; }
        public string SetActName { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string EmpName { get; set; }
        public AssignedTo EmpNameID { get; set; }
        public int EmpID { get; set; }
        public float Counting { get; set; }
        public string Recurrences { get; set; }
        public string BpName { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string AssignedBy { get; set; }
        public int AssignedByID { get; set; }
        public string Percent { get; set; }
        public string Color { get; set; }
        public int data { get; set; }
        public float PerData { get; set; }
        public int CountData { get; set; }
        public float PercentTage { get; set; }
        public int countData { get; set; }
        public string BpCode { get; set; }
        //=========END view report======
        //=========view user=====
        public string UserName { get; set; }
        public string Position { get; set; }
        public string Branch { get; set; }
        public string Action { get; set; }
        //======end view user=====
        public Display GeneralSetting { get; set; }
        public DateTime StartDate { get; set; }


        public string SetupActName { get; set; }
        public int SubNameID { get; set; }
        public int AssignedID { get; set; }
        public string Type { get; set; }

        public int BPID { get; set; }
        public string TelNo { get; set; }
        public bool Personal { get; set; }
        public int ActivityID { get; set; }

        public string Employee { get; set; }
        public string BpType { get; set; }
        public string SubName { get; set; }
        //=====generl===

        public DateTime EndTime { get; set; }
        public string Durration { get; set; }
        public string Priority { get; set; }
        public string Location { get; set; }
        public string StartClock { get; set; }
        public string EndClock { get; set; }

        //===daily===
        public int RepeatDate { get; set; }
        public bool RepeatEveryRecurr { get; set; }
        public bool RepeatEveryWeek { get; set; }
        //==Enddaily====
        //===Weekly==
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }

        //====MONTHLY===
        public int RepeatNumOfmonths { get; set; }
        public bool Days { get; set; }
        public int numDay { get; set; }
        public bool repeatOn { get; set; }
        public string numOfRepeat { get; set; }
        public string DaysInMonthly { get; set; }
        //========yealry======
        public int RepeatofNumAnnualy { get; set; }
        public bool RepeatOncheckYearly { get; set; }
        public string MonthsInAnnualy { get; set; }
        public int NumOfMonths { get; set; }
        public bool checkNumAnnualy { get; set; }
        public string NumofAnnualy { get; set; }
        public string DaysOfAnnualy { get; set; }
        public string MonthsOfAnnulay { get; set; }

        public DateTime Start { get; set; }
        public bool NoEndDate { get; set; }
        public bool After { get; set; }
        public int NumAfter { get; set; }
        public bool By { get; set; }
        public DateTime ByDate { get; set; }

        public List<SelectListItem> AssignedTo { get; set; }
        public List<SelectListItem> Priorities { get; set; }
        public List<SelectListItem> Recurrence { get; set; }
        public List<ContactPersonViewModel> Contact { get; set; }
        public string GetStartHour { get; set; }
        public string GetEndHour { get; set; }
        public int Count { get; set; }
    }
    public class ViewTypeSub
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
    }
}
