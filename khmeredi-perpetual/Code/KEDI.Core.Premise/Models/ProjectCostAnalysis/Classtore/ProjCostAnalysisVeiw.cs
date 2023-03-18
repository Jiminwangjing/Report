using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore
{
    public enum Status { Open = 1, Confirmed = 2, Closed = 3 }
    public enum CopyType
    {
        SQ = 1,
        SDM = 2
    }
    public class ProjCostAnalysisVeiw
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CusID { get; set; }
        public string CusName { get; set; }
        public string CusCode { get; set; }
        public string Phone { get; set; }
        public int ConTactID { get; set; }
        public string ContName { get; set; }
        
        public int BranchID { get; set; }
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
        public DateTime PostingDate { get; set; }
        public DateTime ValidUntilDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public bool IncludeVat { get; set; }
        public Status Status { get; set; }
        public string Remarks { get; set; }
        public int SaleEMID { get; set; }
        public string EmName { get; set; }
        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public double SubTotalBefDis { get; set; }// Subtotal Before Discount
        public double SubTotalBefDisSys { get; set; }// Subtotal Before Discount System
        public double SubTotalAfterDis { get; set; }// Subtotal After Discount
        public double SubTotalAfterDisSys { get; set; }// Subtotal After Discount System
        public double FreightAmount { get; set; }
        public double FreightAmountSys { get; set; }
        public double SubTotal { get; set; }
        public double SubTotalSys { get; set; }
        public double DisRate { get; set; }
        public string DisValue { get; set; }
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
        public FreightProjectCost FreightProjectCost { get; set; }
    }
}
