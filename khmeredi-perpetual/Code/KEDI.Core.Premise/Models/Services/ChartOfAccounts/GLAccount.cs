using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sync;

namespace CKBS.Models.Services.ChartOfAccounts
{
    public enum AccountType { Other = 0, Sale = 1, Expenditure = 2 };
    [Table("tbGLAccount")]
    public class GLAccount : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int SubTypeAccountID { get; set; }
        [Required]
        public string Code { get; set; }
        public string ExternalCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
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
        public int ParentId { get; set; }
        public int MainParentId { get; set; }
        [NotMapped]
        public string TotalBalance { get; set; }
        [NotMapped]
        public double FormatNumber { get; set; }
        [NotMapped]
    
        public bool Edit { get; set; }
        public int CompanyID { get; set; }
        public CategoryType CategoryType { get; set; }

        //ISyncEntity
        public Guid RowId { get; set; }
        public DateTimeOffset ChangeLog { get; set; }
    }
    public enum CategoryType
    {
        Assets = 1,
        Liabilities = 2,
        CapitalReserves = 3,
        Turnover = 4,
        CostofSales = 5,
        OperatingCosts = 6,
        NOIE = 7,//Non-Operating Income & Expenditure
        TaxationExtraordinary = 8, //Taxation & Extraordinary Items
    }

}
