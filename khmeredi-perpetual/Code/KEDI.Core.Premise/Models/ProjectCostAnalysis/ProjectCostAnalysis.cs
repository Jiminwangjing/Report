using KEDI.Core.Premise.Models.Sale;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis
{
    public enum Status { Open = 1, Confirmed= 2, Closed = 3 }
    [Table("tbProjectCostAnalysis")]
    public class ProjectCostAnalysis
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int CusID { get; set; }
        public int ConTactID { get; set; }
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
        public DateTime ValidUntilDate { get; set; }
        [NotMapped]
        public DateTime DeliveryDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public bool IncludeVat { get; set; }
        public Status Status { get; set; }
        public string Remarks { get; set; }
        public int SaleEMID { get; set; }
        public int OwnerID { get; set; }
        public double SubTotalBefDis { get; set; }// Subtotal Before Discount
        public double SubTotalBefDisSys { get; set; }// Subtotal Before Discount System
        public double SubTotalAfterDis { get; set; }// Subtotal After Discount
        public double SubTotalAfterDisSys { get; set; }// Subtotal After Discount System
        public double FreightAmount { get; set; }
        public double FreightAmountSys { get; set; }
        [Required]
        public double SubTotal { get; set; }
        [Required]
        public double SubTotalSys { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double TotalAmount { get; set; }
        public double TotalAmountSys { get; set; }
        public DateTime ChangeLog { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public double TotalMargin { get; set; }
        public double TotalCommission { get; set; }
        public double OtherCost { get; set; }
        public double ExpectedTotalProfit { get; set; }
        public int BaseOnID { get; set; }
        public CopyType CopyType { get; set; }
        public string KeyCopy { get; set; }

        [NotMapped]
        public FreightProjectCost FreightProjectCost { get; set; }
        public IEnumerable<ProjCostAnalysisDetail> ProjCostAnalysisDetails { get; set; }
    }
   public enum CopyType
    {
        SQ=1,
        SDM=2
    }
}
