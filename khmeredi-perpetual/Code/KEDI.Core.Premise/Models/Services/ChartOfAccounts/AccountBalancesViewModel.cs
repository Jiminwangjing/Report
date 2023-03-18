using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ChartOfAccounts
{
    public class AccountBalancesViewModel
    {
        public int LineID { get; set; }
        public int ID { get; set; }
        public string PostingDate { get; set; }      
        public string Code { get; set; }
        public int Origin { get; set; }
        public string OriginNo { get; set; }
        public string OffsetAccount { get; set; }
        public string Details { get; set; }
        public string CumulativeBalance { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public decimal LocalSetRate { get; set; }
        public int GLAID { get; set; }
        
    }
}
