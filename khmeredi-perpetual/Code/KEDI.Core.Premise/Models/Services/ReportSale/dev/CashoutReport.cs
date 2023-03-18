using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    [Table("rp_Cashout", Schema = "dbo")]
    public class CashoutReport
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        [NotMapped]
        public string Barcode { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double DisItemValue { get; set; }
        public double Total { get; set; }
        public string Currency { get; set; }
        public string Currency_Sys { get; set; }
        public double TotalSoldAmount { get; set; }
        public double TotalDiscountItem { get; set; }
        public double TotalDiscountTotal { get; set; }
        public double TotalVat { get; set; }
        public double GrandTotal { get; set; }
        public double GrandTotal_Sys { get; set; }
        public double TotalCashIn_Sys { get; set; }
        public double TotalCashOut_Sys { get; set; }
        public double ExchangeRate { get; set; }
        public double GrandTotal_Display { get; set; }
        public double ExchangeRate_Display { get; set; }
        public string CurrencyDisplay { get; set; }

        public string ItemGroup1 { get; set; }
        public string ItemGroup2 { get; set; }
        public string ItemGroup3 { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string EmpName { get; set; }
        public string Uom { get; set; }
        public int ItemId { get; set; }
    }


    public class CashoutReportMaster
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
        public string DisItemValue { get; set; }
        public string Total { get; set; }
        public string Currency { get; set; }
        public string Currency_Sys { get; set; }
        public string TotalSoldAmount { get; set; }
        public string TotalDiscountItem { get; set; }
        public string TotalDiscountTotal { get; set; }
        public string TotalVat { get; set; }
        public string GrandTotal { get; set; }
        public string GrandTotal_Sys { get; set; }
        public string TotalCashIn_Sys { get; set; }
        public string TotalCashOut_Sys { get; set; }
        public string ExchangeRate { get; set; }
        public string GrandTotal_Display { get; set; }
        public string ExchangeRate_Display { get; set; }
        public string CurrencyDisplay { get; set; }
        public string ReceiptNo { get; set; }
        public string Amount { get; set; }

        public string ItemGroup1 { get; set; }
        public string ItemGroup2 { get; set; }
        public string ItemGroup3 { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string EmpName { get; set; }
        public string Uom { get; set; }
        public double TotalCal { get; set; }
        public bool IsNone { get; set; }
        public string TotalCredites { get; set; }
    }
}
