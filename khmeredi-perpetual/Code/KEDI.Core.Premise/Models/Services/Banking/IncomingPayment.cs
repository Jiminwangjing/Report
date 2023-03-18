using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbIncomingPayment", Schema = "dbo")]
    public class IncomingPayment
    {
        [Key]
        public int IncomingPaymentID { get; set; }
        public int UserID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public int CompanyID { get; set; }
        public int PaymentMeanID { get; set; }
        public string InvoiceNumber { get; set; }
        public int CustomerID { get; set; }
        public int BranchID { get; set; }
        public string Ref_No { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public List<IncomingPaymentDetail> IncomingPaymentDetails { get; set; }
        public List<MultiIncomming> MultiIncommings { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("CustomerID ")]
        public BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int IcoPayCusID { get; set; }
        public decimal TotalApplied { get; set; }
        public int BasedOnID { get; set; }
        public ConpyType CopyType { get; set; }
    }
    public enum ConpyType {
        None=0,
        IncomingPaymentOrder=1,

    }
}
