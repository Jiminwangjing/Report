using System.Collections.Generic;
using System.Linq;

namespace KEDI.Core.Premise.Models.ServicesClass.KSMS
{
    public class SaleReportKSMS
    {
        public string LineID { get; set; }
        public int RID { get; set; }
        public int CusID { get; set; }
        //Master
        public string CusName { get; set; }
        public string Plate { get; set; }
        public string ModelName { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
        public string Cur { get; set; }
        // Summary
        public int TotalItem { get; set; }
        public string SoldAmount { get; set; }
        public string GrandTotal { get; set; }
        public string TotalAll { get; set; }
        public string TotalInvoice { get; set; }
        public string PostingDate { get; set; }

        // detial
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string UoM { get; set; }
        public string Discount { get; set; }
        public double Total { get; set; }
        public string TotalF { get; set; }
        public string Invoice { get; set; }
        public string UnitPrice { get; set; }
    }

    public class SaleReportKSMSHeader
    {
        public string ToDate { get; set; }
        public string FromDate { get; set; }
    }
    public class SaleReportKSMSFooter
    {
        public int TotalItem { get; set; }
        public string TotalAll { get; set; }
    }
    public class SaleReportKSMSPrint
    {
        public SaleReportKSMSHeader Header { get; set; }
        public SaleReportKSMSFooter Footer { get; set; }
        public List<IGrouping<int, SaleReportKSMS>> Detials { get; internal set; }
    }
}
