using CKBS.Models.Services.Administrator.SystemInitialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Financials
{
    [Table("tbJournalEntry", Schema = "dbo")]
    public class JournalEntry
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please select Series !")]
        public int SeriesID { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public int DouTypeID { get; set; }
        [Required]
        public int Creator { get; set; }
        [Required]
        public string TransNo { get; set; }
        [Required(ErrorMessage = "Please select postingdate")]
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Required(ErrorMessage = "Please select duedate")]
        [Column(TypeName = "Date")]
        public DateTime DueDate { get; set; }
        [Required(ErrorMessage = "Please select documentdate")]
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        [Required(ErrorMessage = "Please input remarks")]
        public string Remarks { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int SSCID { get; set; }
        public int LLCID { get; set; }
        public decimal LocalSetRate { get; set; }
        public int SeriesDID { get; set; }
        public int CompanyID { get; set; }
        public List<Series> Series { get; internal set; }
        public IEnumerable<JournalEntryDetail> JournalEntryDetails { get; set; }
        [NotMapped]
        public string SysCurrency { get; set; }
        public int BranchID {get;set;}
    }
}
