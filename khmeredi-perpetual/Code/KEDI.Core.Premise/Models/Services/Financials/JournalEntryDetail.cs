using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Financials
{    
    [Table("tbJournalEntryDetail", Schema = "dbo")]
    public class JournalEntryDetail
    {
        [NotMapped]
        public int LineID { get; set; }
        [NotMapped]
        public string CodeTM { get; set; }
        [NotMapped]
        public string NameTM { get; set; }
        [NotMapped]
        public string DebitTM { get; set; }
        [NotMapped]
        public string CreditTM { get; set; }
        public int ID { get; set; }        
        public int JEID { get; set; }
        public Type Type { get; set; }
        [Required(ErrorMessage = "Please choose G/L Acct")]
        public int ItemID { get; set; } //GLAcctID or BPID
        [Required(ErrorMessage = "Please input Dedit")]
        public decimal Debit { get; set; }
        [Required(ErrorMessage = "Please input Credit")]
        public decimal Credit { get; set; }
        public string Remarks { get; set; }
        public int BPAcctID  { get; set; }        
    }
    public enum Type
    {
        GLAcct=1, BPCode=2
    }
}
