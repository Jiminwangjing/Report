using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Activity
{
    [Table("General")]
    public class General
    {
        [Key]

        public int ID { get; set; }
        public string Remark { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Durration { get; set; }
        public int StatusID { get; set; }
        public string Recurrence { get; set; }
        public string Priority { get; set; }
        public string Location { get; set; }
        //public string StartClock { get; set; }
        //public string EndClock { get; set; }
        public int ActivityID { get; set; }
        //==daily====
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
        //=====end weekly==
        //public int RepeatOn { get; set; }
        //public bool Day1  { get; set; }
        //public bool Day2  { get; set; }
        //public string Num  { get; set; }
        //public string Week  { get; set; }
        //========monthly=======
        public int RepeatNumOfmonths { get; set; }
        public bool Days { get; set; }
        public int numDay { get; set; }
        public bool repeatOn { get; set; }
        public string numOfRepeat { get; set; }
        public string DaysInMonthly { get; set; }
        //====end monthly=======
        //======yearly=========
        public int RepeatofNumAnnualy { get; set; }
        public bool RepeatOncheckYearly { get; set; }
        public string MonthsInAnnualy { get; set; }
        public int NumOfMonths { get; set; }
        public bool checkNumAnnualy { get; set; }
        public string NumofAnnualy { get; set; }
        public string DaysOfAnnualy { get; set; }
        public string MonthsOfAnnulay { get; set; }
        //===end yearly========
        public DateTime Start  { get; set; }
        public bool NoEndDate  { get; set; }
        public bool After  { get; set; }
        public int NumAfter  { get; set; }
        public bool By  { get; set; }
        public DateTime ByDate  { get; set; }

        //public bool EveryWeekDay  { get; set; }
        //public bool Mon  { get; set; }
        //public bool Tue  { get; set; }
        //public bool Wed  { get; set; }
        //public bool Thu  { get; set; }
        //public bool Fri  { get; set; }
        //public bool Sat  { get; set; }
        //public bool Sun  { get; set; }
        //public bool Months  { get; set; }
        //public string SelectMonths  { get; set; }
        //public int DayInMonth  { get; set; }
    }
}
