using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    [Table("tbOutgoingpaymentOrder",Schema ="dbo")]
    public class OutgoingPaymentOrder
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        [NotMapped]
        public string UserName{get;set;}
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public int DocumentID { get; set; }
        public int CompanyID { get; set; }
        public string NumberInvioce { get; set; }
        [NotMapped]
        public string DocumentCode{get;set;}
        public string Status { get; set; }
        public int PaymentMeanID { get; set; }
        [NotMapped]
        public string PaymentMeanName{get;set;}
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        [NotMapped]
        public string VendorName{get;set;}
        public string Ref_No { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
        [NotMapped]
        public double TotalPayment{get;set;}
        public string Number { get; set; }
        public string Remark { get; set; }
        public List<OutgoingPaymentOrderDetail> OutgoingPaymentOrderDetail { get; set; }

        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("VendorID")]
        public  BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public TypePurchase TypePurchase { get; set; }

    }

}