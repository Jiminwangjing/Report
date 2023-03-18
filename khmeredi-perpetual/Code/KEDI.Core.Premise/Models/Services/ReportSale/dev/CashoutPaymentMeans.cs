using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    [Table("rp_CashoutPaymentMeans", Schema = "dbo")]
    public class CashoutPaymentMeans
    {
        [Key]
        public int ID { get; set; }
        public string ReceiptNo { get; set; }
        public double DisItemValue { get; set; }
        public double Amount { get; set; }
        public string PaymentMeans { get; set; }
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
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string EmpName { get; set; }
        public string ReceiptTime { get; set; }
        public string ReceiptDate { get; set; }

    }

    public class CashoutPaymentMeansMaster
    {
        public int ID { get; set; }
        public string ReceiptNo { get; set; }
        public string DisItemValue { get; set; }
        public string Amount { get; set; }
        public string PaymentMeans { get; set; }
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
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string EmpName { get; set; }
        public string ReceiptTime { get; set; }
        public string ReceiptDate { get; set; }
        public bool IsNone { get; set; }
    }
}
