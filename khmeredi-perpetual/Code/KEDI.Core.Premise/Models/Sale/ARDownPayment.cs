using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("ARDownPayment")]
    public class ARDownPayment
    {
        [Key]
        public int ARDID { get; set; }
        public int CusID { get; set; }
        public int RequestedBy { get; set; }
        public int ShippedBy { get; set; }
        public int ReceivedBy { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int SaleCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public string InvoiceNumber { get; set; }
        public string RefNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal ExchangeRate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DueDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ValidUntilDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public string Status { get; set; }
        [Required]
        public double Total { get; set; }
        [Required]
        public double SubTotal { get; set; }
        [Required]
        public double SubTotalSys { get; set; }
        public decimal SubTotalBefDis { get; set; }// Subtotal Before Discount
        public decimal SubTotalBefDisSys { get; set; }// Subtotal Before Discount System
        public decimal SubTotalAfterDis { get; set; }// Subtotal After Discount
        public decimal SubTotalAfterDisSys { get; set; }// Subtotal After Discount System
        public decimal DPMRate { get; set; }
        public decimal DPMValue { get; set; }
        public decimal DisRate { get; set; }
        public decimal DisValue { get; set; }
        public decimal VatValue { get; set; }
        public decimal VatRate { get; set; }
        public string TypeDis { get; set; }
        public double TotalAmount { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal BalanceDueSys { get; set; }
        public decimal AppliedAmountSys { get; set; }
        public double TotalAmountSys { get; set; }
        public SaleCopyType CopyType { get; set; }
        public string CopyKey { get; set; }
        public string BasedCopyKeys { get; set; }
        public DateTime ChangeLog { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurID { get; set; }
        public decimal LocalSetRate { get; set; }
        public string Remarks { get; set; }
        public int ARID { get; set; }
        public int SaleEmID { get; set; }
        
        public IEnumerable<ARDownPaymentDetail> ARDownPaymentDetails { get; set; }
    }
}
