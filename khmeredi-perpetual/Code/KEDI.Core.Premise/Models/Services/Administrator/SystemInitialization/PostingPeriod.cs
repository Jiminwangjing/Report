using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    [Table("tbPotingPeriod", Schema ="dbo")]
    public class PostingPeriod
    {
        [Key]
        public int ID { get; set; }
        public string PeriodCode { get; set; }
        public string PeriodName { get; set; }
        public SubPeriod SubPeriod { get; set; }
        public string NoOfPeroid { get; set; }
        public int PeroidIndID { get; set; }
        public string Category { get; set; }
        public PeroidStatus PeroidStatus { get; set; }
        [Required(ErrorMessage = "Please select posting date from")]
        [Column(TypeName = "Date")]
        public DateTime PostingDateFrom { get; set; }
        [Required(ErrorMessage = "Please select posting date to")]
        [Column(TypeName = "Date")]
        public DateTime PostingDateTo { get; set; }
        [Required(ErrorMessage = "Please select due date from")]
        [Column(TypeName = "Date")]
        public DateTime DueDateFrom { get; set; }
        [Required(ErrorMessage = "Please select due date to")]
        [Column(TypeName = "Date")]
        public DateTime DueDateTo { get; set; }
        [Required(ErrorMessage = "Please select document date from")]
        [Column(TypeName = "Date")]
        public DateTime DocuDateFrom { get; set; }
        [Required(ErrorMessage = "Please select document date to")]
        [Column(TypeName = "Date")]
        public DateTime DocuDateTo { get; set; }
        [Required(ErrorMessage = "Please select duedate from")]
        [Column(TypeName = "Date")]
        public DateTime StartOfFiscalYear { get; set; }
        public string FiscalYear { get; set; }
        public int CompanyID { get; set; }
    }
    public enum SubPeriod
    {
        None = 0,
        Year = 1,
        Month = 2
        
    }
    public enum PeroidStatus
    {
        Unlocked =1,
        UnlockedExceptSale=2,
        ClosingPeriod=3,
        Locked=4
    }
}
