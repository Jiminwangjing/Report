using System.Collections.Generic;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupSummarySale
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public double SubTotal { get; set; }
        public List<Receipts> Receipts { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }
        public string LCC { get; set; }
        public string SC { get; set; }
    }
    public class Receipts
    {
        public string User { get; set; }
        public string Receipt { get; set; }
        public string DateIn { get; set; }
        public string TimeIn { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public double DisInv { get; set; }
        public string Currency { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalGrouptPayment { get; set; }
        public double GrandTotal { get; set; }
        public string LocalCurrency { get; set; }
        public string Customer { get; set; }
        public string Remark { get; set; }
    }
    public class Header
    {
        public string Logo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Branch { get; set; }
        public string EmpName { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }

    }
    public class Footer
    {
        public string TotalAppliedAmountGroup { get; set; }
        public string TotalBalanceDueGroup { get; set; }
        public string TotalSumGroup { get; set; }
        public string CountReceipt { get; set; }
        public string SoldAmount { get; set; }
        public string DiscountItem { get; set; }
        public string DiscountTotal { get; set; }
        public string TaxValue { get; set; }
        public string GrandTotal { get; set; }
        public string GrandTotalSys { get; set; }
        public int CountReceiptCal { get; set; }
        public double DiscountItemCal { get; set; }
        public double DiscountTotalCal { get; set; }
        public double TaxValueCal { get; set; }
        public double GrandTotalCal { get; set; }
        public double GrandTotalSysCal { get; set; }


        //===============//
        public string  GrandTotalBrand {get;set;}
         public string SubTotal {get;set;}
         public string Total {get;set;}
         public string SDiscountItem {get;set;}
         public string SDiscountTotal {get;set;}
         public string SVat {get;set;}
         public string SGrandTotalSys {get;set;}
         public string SGrandTotal {get;set;}
        public double SubTotalCal { get; set; }
        public double PriceCal { get; set; }
        public double ReturnQty { get; set; }


    }
 
}
