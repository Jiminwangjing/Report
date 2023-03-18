using System;
using System.Collections.Generic;
using CKBS.Models.Services.ChartOfAccounts;

namespace CKBS.Models.ServicesClass
{
    public class GLAccountBalanceSheetreportViewModelForTB
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
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public List<AccountBalance> AccountBalances { get; set; }
        public List<AccountBalance> ActiveAccountBalances { get; set; }
        public decimal Sum { get; set; }
        /// <summary>
        ///  Display Quarter
        /// </summary>
        /// <summary>
        ///  Display Quarter
        /// </summary>
        public decimal CQ1 { get; set; }
        public decimal DQ1 { get; set; }
        public decimal BQ1 { get; set; }
        public decimal CQ2 { get; set; }
        public decimal DQ2 { get; set; }
        public decimal BQ2 { get; set; }
        public decimal CQ3 { get; set; }
        public decimal DQ3 { get; set; }
        public decimal BQ3 { get; set; }
        public decimal CQ4 { get; set; }
        public decimal DQ4 { get; set; }
        public decimal BQ4 { get; set; }

        /// <summary>
        /// Display Monthly
        /// </summary>
        public decimal CM1 { get; set; }
        public decimal DM1 { get; set; }
        public decimal BM1 { get; set; }
        public decimal BM2 { get; set; }
        public decimal DM2 { get; set; }
        public decimal CM2 { get; set; }
        public decimal CM3 { get; set; }
        public decimal DM3 { get; set; }
        public decimal BM3 { get; set; }
        public decimal CM4 { get; set; }
        public decimal DM4 { get; set; }
        public decimal BM4 { get; set; }
        public decimal CM5 { get; set; }
        public decimal DM5 { get; set; }
        public decimal BM5 { get; set; }
        public decimal CM6 { get; set; }
        public decimal DM6 { get; set; }
        public decimal BM6 { get; set; }
        public decimal CM7 { get; set; }
        public decimal DM7 { get; set; }
        public decimal BM7 { get; set; }
        public decimal CM8 { get; set; }
        public decimal DM8 { get; set; }
        public decimal BM8 { get; set; }
        public decimal BM9 { get; set; }
        public decimal DM9 { get; set; }
        public decimal CM9 { get; set; }
        public decimal CM10 { get; set; }
        public decimal DM10 { get; set; }
        public decimal BM10 { get; set; }
        public decimal CM11 { get; set; }
        public decimal DM11 { get; set; }
        public decimal BM11 { get; set; }
        public decimal CM12 { get; set; }
        public decimal DM12 { get; set; }
        public decimal BM12 { get; set; }
    }
}


