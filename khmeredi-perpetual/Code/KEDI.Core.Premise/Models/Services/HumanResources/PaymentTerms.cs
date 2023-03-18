using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbPaymentTerms", Schema = "dbo")]

    public class PaymentTerms
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public StartFrom? StartFrom { get; set; }
        public DueDate? DueDate { get; set; }
        public OpenIncomingPayment? OpenIncomingPayment { get; set; }
        public string TolerenceDay { get; set; }
        public float TotalDiscount { get; set; }
        public float InterestOnReceiVables {get;set;}
        public float MaxCredit { get; set; }
        public float CommitLimit { get; set; }
        public int PriceListID { get; set; }
        public int InstaillmentID { get; set; }
        public int CashDiscountID { get; set; }
        //public int BusinessPartnerID { get; set; }

    }

    public enum StartFrom
    {
        None = 0,
        MonthStart = 1,
        HaftMonth = 2,
        EndMonth = 3,
    }

    public enum DueDate
    {
        None = 0,
        SystemDate = 1,
        PostingDate = 2,
        DocumentDate = 3,
    }
    public enum OpenIncomingPayment
    {
        None= 0,
        No= 1,
        Cash = 2,
        Checks = 3,
        Credit=4,
        BinkTransfer=5
    }
}
