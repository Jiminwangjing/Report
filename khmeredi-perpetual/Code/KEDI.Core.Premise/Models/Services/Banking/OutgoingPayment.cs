using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Banking
{
    [Table("tbOutgoingpayment",Schema ="dbo")]
    public class OutgoingPayment
    {
        [Key]
        public int OutgoingPaymentID { get; set; }
        public int UserID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public int DocumentID { get; set; }
        public int CompanyID { get; set; }
        public string NumberInvioce { get; set; }
        public string Status { get; set; }
        public int PaymentMeanID { get; set; }
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        public string Ref_No { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public List<OutgoingPaymentDetail> OutgoingPaymentDetails { get; set; }

        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("VendorID")]
        public  BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public TypePurchase TypePurchase { get; set; }
        public int  BaseOnID{get;set;}

    }
    public enum TypePurchase
    {
        AP = 1,
        APReserve = 2,
        CreditMemo=3,
        OutgoingPayOrder=4,
    }
}
