using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Financials;

namespace KEDI.Core.Premise.Models.ServicesClass.FinancialReports
{
    public class TransactionJournalReport
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public string Series { get; set; }
        public string MasterRemarks { get; set; }
        public string Remarks { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string Trans { get; set; }
        public string Creator { get; set; }
        public string AccountBPCode { get; set; }
        public string AccountBPName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string TotalCrebit { get; set; }
        public string CreditF { get; set; }
        public string DebitF { get; set; }
        public string TotalDebit { get; set; }
        public string TotalGroupDebit { get; set; }
        public string TotalGroupCredit { get; set; }
        public int ABID { get; set; }
        public int SeriesID { get; set; }
        public int GlID { get; set; }
        public int Efective { get; set; }
        public List<JournalEntryDetail> JournalEntryDetail { get; set; }
    }
}
