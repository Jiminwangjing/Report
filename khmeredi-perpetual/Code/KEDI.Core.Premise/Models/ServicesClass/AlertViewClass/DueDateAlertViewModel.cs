using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.Models.Sale;
using System;
using System.Collections.Generic;

namespace CKBS.Models.ServicesClass.AlertViewClass
{
    public class DueDateAlertViewModel
    {
        public int ID { get; set; }
        public int BPID { get; set; }
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DueDateType DueDateType { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string TimeLeft { get; set; }
        public int SeriesDID { get; set; }
    }
    public class DueDateViewModel
    {
        public string EmpName { get; set; }
        public string BrandName { get; set; }
        public string Currency { get; set; }
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
        public DateTime DueDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public double SubTotal { get; set; }
        public double SubTotalSys { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double AppliedAmount { get; set; }
        public string FeeNote { get; set; }
        public double FeeAmount { get; set; }
        public double TotalAmount { get; set; }
        public double TotalAmountSys { get; set; }
        public DateTime ChangeLog { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public IEnumerable<SaleARDetail> SaleARDetails { get; set; }
    }
    public class DueDateDetailViewModel
    {
        public string ItemCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PrintQty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public double Factor { get; set; }
        public double Cost { get; set; } = 0;
        public double UnitPrice { get; set; }
        public double DisRate { get; set; }
        public double DisValue { get; set; }
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public int CurrencyID { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public string Remarks { get; set; }
        public bool Delete { get; set; }
    }
}
