using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Report
{
    public class DevGroupCustomer
    {
        public string GroupName { get; set; }
        public string GrandTotalCustomer { get; set; }
        public string ApplyAmountTotal { get; set; }
        public string DouType { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string CustName { get; set; }
        public int CustID { get; set; }
        public string ReceiptNo { get; set; }
        public decimal ApplyAmount { get; set; }
        public double BalanceDue { get; set; }
        public int ReceiptID { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public string DiscountItem { get; set; }
        public string Currency { get; set; }
        public string GrandTotal { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string TotalSumGroup { get; set; }
        public string TotalAppliedAmountGroup { get; set; }
        public string TotalBalanceDueGroup { get; set; }
        public double GrandTotalSys { get; set; }
        public double MGrandTotal { get; set; }
        public string RefNo { get; set; }
        public string DisRemark { get; set; }
        public int Group1ID { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal GrandTotalCal { get; set; }
        public decimal AppliedAmountCal { get; set; }
        public string BalanceDueTotal { get; set; }
       
    }
}
