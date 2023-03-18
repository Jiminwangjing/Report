using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking
{
    [Table("tbIncomingPaymentCustomer", Schema = "dbo")]
    public class IncomingPaymentCustomer
    {
        [Key]
        public int IncomingPaymentCustomerID { get; set; }
        public int CustomerID { get; set; }
         [NotMapped]
        public string CustomerName{get;set;}
           [NotMapped]
        public string CustomerNametwo {get;set;}
         [NotMapped]
        public string ContactName {get;set;}
        [NotMapped]
        public string EmployeeName{get;set;}
     
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public int CompanyID { get; set; }
        public string InvoiceNumber { get; set; }
        public string ItemInvoice { get; set; }
        public int BranchID { get; set; }
        public int WarehouseID { get; set; }
        public int CurrencyID { get; set; }
        public string DocumentNo { get; set; }
        public string DocumentType { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [NotMapped]
        public string DateFrom{get;set;}
        [NotMapped]
        public string DateTo{get;set;}
         [NotMapped]
        public double BalanceDueSSC{get;set;}
         [NotMapped]
        public double BalanceDueLC{get;set;}
        
        public double OverdueDays { get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double TotalPayment { get; set; }
        public double Applied_Amount { get; set; }
        public string CurrencyName { get; set; }
        public string SysName { get; set; }
        public string Status { get; set; }
        public double CashDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double ExchangeRate { get; set; }
        public int SysCurrency { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int EmID{get;set;}
        public string EmName{get;set;}
        public int CreatorID{get;set;}
        public string CreatorName{get;set;}
        public string Types { get; set; }
        [NotMapped]
        public List<DetailItemd> DetailIteme {get;set;}

    }

     public class DetailItemd
    {
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string Code { get; set; }
        [NotMapped]
        public double Qty { get; set; }
        [NotMapped]
        public double Price { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        public double TotalAmount { get; set; }
    }
}

