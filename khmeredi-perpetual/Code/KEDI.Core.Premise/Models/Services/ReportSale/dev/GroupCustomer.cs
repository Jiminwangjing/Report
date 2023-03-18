using CKBS.Models.Services.ReportSale.dev;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.ReportSale.dev
{
    public class GroupCustomer
    {
        public string GroupName { get; set; }
        public int ReceiptID { get; set; }
        public string EmpName { get; set; }
        public string ReceiptNo { get; set; }
        public string DiscountItem { get; set; }
        public string GrandTotalStr { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public int Group1ID { get; set; }
        public string TotalSumGroup { get; set; }
        public string TotalAppliedAmountGroup { get; set; }
        public string TotalBalanceDueGroup { get; set; }
        public decimal AppliedAmountCal { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Currency { get; set; }
        public decimal GrandTotalCal { get; set; }
        public string BalanceDueTotal { get; set; }
        public string ApplyAmountTotal { get; set; }
        public double BalanceDue { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }

        public List<GroupItem> GroupItems { get; set; }
        public string GrandTotalCustomer { get; set; }
        public int Decimalplaces { get; set; }
    }
    public class GroupItem
    {
        public string Currency { get; set; }
        public string EmpName { get; set; }
        public string ReceiptNo { get; set; }
        public string TimeOut { get; set; }
        public double DisInvoice { get; set; }
        public string DiscountItem { get; set; }
        public string DateOut { get; set; }
        public string DisRemark { get; set; }
        public string PosDate { get; set; }
        public decimal ApplyAmount { get; set; }
        public double BalanceDue { get; set; }
        public string Code { get; set; }
        public string CustName { get; set; }
        public int CustID { get; set; }
        public string Doutype { get; set; }

        public double Total { get; set; }
        public string TotalStr { get; set; }
        public string EnglisName { get; set; }
    }
}
