using KEDI.Core.Premise.Models.ServicesClass.Sale;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("tbSaleOrder", Schema = "dbo")]
    public class SaleOrder
    {
        [Key]
        public int SOID { get; set; }
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
        public double ExchangeRate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DeliveryDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public bool IncludeVat { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public decimal SubTotalBefDis { get; set; }// Subtotal Before Discount
        public decimal SubTotalBefDisSys { get; set; }// Subtotal Before Discount System
        public decimal SubTotalAfterDis { get; set; }// Subtotal After Discount
        public decimal SubTotalAfterDisSys { get; set; }// Subtotal After Discount System
        public decimal FreightAmount { get; set; }
        public decimal FreightAmountSys { get; set; }
        [Required]
        public double SubTotal { get; set; }
        [Required]
        public double SubTotalSys { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public string FeeNote { get; set; }
        public double FeeAmount { get; set; }
        public double TotalAmount { get; set; }
        public double TotalAmountSys { get; set; }
        public SaleCopyType CopyType { get; set; }
        public string CopyKey { get; set; }
        public string BasedCopyKeys { get; set; }
        public DateTime ChangeLog { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int SaleEmID { get; set; }
        public int BaseOnID { get; set; }
        [NotMapped]
        public FreightSale FreightSalesView { get; set; }
        public IEnumerable<SaleOrderDetail> SaleOrderDetails { get; set; }
    }
}
