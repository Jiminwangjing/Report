using System;

namespace CKBS.Models.ServicesClass
{
    public class CashFlowForTreasuryViewModel
    {
        public int ID { get; set; }
        public string DueDate { get; set; }
        public string Origin { get; set; }
        public string Referrence { get; set; }
        public string ControlAccount { get; set; }
        public string GLAccBPCode { get; set; }
        public string Remarks { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
        public string Total { get; set; }
        public string CreditTotal { get; set; }
        public string DebitTotal { get; set; }
        public string TotalSummary { get; set; }
        public string BalanceTotal { get; set; }
    }
}
