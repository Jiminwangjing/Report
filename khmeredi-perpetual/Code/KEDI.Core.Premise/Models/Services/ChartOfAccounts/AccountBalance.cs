using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ChartOfAccounts
{
    [Table("tbAccountBalance")]
    public class AccountBalance
    {
        [Key]
        public int ID { get; set; }
        public int JEID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        public int Origin { get; set; }
        public string OriginNo { get; set; }
        public string OffsetAccount { get; set; }
        public string Details { get; set; }
        public decimal CumulativeBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal LocalSetRate { get; set; }
        public int GLAID { get; set; }
        public string Remarks { get; set; }
        public int BPAcctID { get; set; }
        public int Creator { get; set; }
        [NotMapped]
        public int ParentID { get; set; }
        public EffectiveBlance Effective { get; set; }

    }
    public enum EffectiveBlance
    {
        Debit = 1,
        Credit = 2
    }
}
