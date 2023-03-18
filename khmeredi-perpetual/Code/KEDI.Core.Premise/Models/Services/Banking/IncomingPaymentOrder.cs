using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    public class IncomingPaymentOrder
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public int CompanyID { get; set; }
        public int PaymentMeanID { get; set; }
        public string InvoiceNumber { get; set; }
        public int CustomerID { get; set; }
         public string Type { get; set; }
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
        public List<IncomingPaymentOrderDetail> IncomingPaymentOrderDetails { get; set; }
        public List<MultiIncomingPaymentOrder> MultiIncomingPaymentOrders { get; set; }
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
    }
}
