using System;
using System.Collections.Generic;
using CKBS.Models.Services.ChartOfAccounts;

namespace CKBS.Models.ServicesClass
{
    public class GLAccountBalanceSheetreportViewModel
    {
        public int ID { get; set; }
        public int MainParentID { get; set; } 
        public string Code { get; set; }
        public string ExternalCode { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool IsTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfidential { get; set; }
        public bool IsIndexed { get; set; }
        public bool IsCashAccount { get; set; }
        public bool IsControlAccount { get; set; }
        public bool BlockManualPosting { get; set; }
        public bool CashFlowRelavant { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public DateTimeOffset ChangeLog { get; set; }
        public int ParentId { get; set; }
        public int CompanyID { get; set; }
        public AccountBalance AccountBalance { get; set; }
        public List<AccountBalance> AccountBalances { get; set; }
        public decimal Sum { get; set; }
        /// <summary>
        ///  Display Quarter
        /// </summary>
        public decimal Q1 { get; set; }
        public decimal Q2 { get; set; }
        public decimal Q3 { get; set; }
        public decimal Q4 { get; set; }

        /// <summary>
        /// Display Monthly
        /// </summary>
        public decimal M1 { get; set; }
        public decimal M2 { get; set; }
        public decimal M3 { get; set; }
        public decimal M4 { get; set; }
        public decimal M5 { get; set; }
        public decimal M6 { get; set; }
        public decimal M7 { get; set; }
        public decimal M8 { get; set; }
        public decimal M9 { get; set; }
        public decimal M10 { get; set; }
        public decimal M11 { get; set; }
        public decimal M12 { get; set; }
        public decimal FormatNumber { get; set; }
    }

}
