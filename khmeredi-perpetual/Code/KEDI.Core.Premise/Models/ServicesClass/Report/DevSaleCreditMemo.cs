using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.Report
{
    public class DevSaleCreditMemo
    {
        public string GrandTotalBrand { get; set; }
        public int  DouTypeID{get;set;}
        public string DouType { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string BranchName { get; set; }
        public int BranchID { get; set; }
        public string ReceiptNo { get; set; }
        public int ReceiptID { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public string DiscountItem { get; set; }
        public string Currency { get; set; }
        public string GrandTotal { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string SCount { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SVat { get; set; }
        public string SGrandTotalSys { get; set; }
        public string SGrandTotal { get; set; }
        public decimal TotalDiscountItem { get; set; }
        public double DiscountTotal { get; set; }
        public double Vat { get; set; }
        public double GrandTotalSys { get; set; }
        public double MGrandTotal { get; set; }
        public int ReceiptKvmsID { get; set; }
        public int MReceiptID { get; set; }


        public string MReceiptNo { get; set; }
        public string MUserName { get; set; }
        public string MDateOut { get; set; }
        public string MVat { get; set; }
        public string MDisTotal { get; set; }
        public string MSubTotal { get; set; }
        public string MTotal { get; set; }
        public int ID { get; set; }
        public string ItemCode { get; set; }
        public string KhmerName { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public string UnitPrice { get; set; }
        public string DisItem { get; set; }
        public string Total { get; set; }
         public string ItemNameKH { get; set; }
        public string InvoiceNo { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } 
        public string DiscountValue { get; set; }
        public double Totals { get; set; }
        public double UnitPrices { get; set; }
        public double DiscountValuse { get; set; }

    }
}
