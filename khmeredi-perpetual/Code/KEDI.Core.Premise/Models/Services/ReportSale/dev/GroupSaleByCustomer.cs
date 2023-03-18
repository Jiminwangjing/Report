using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupSaleByCustomer
    {
        public string CusName { get; set; }
        public double SubCusTotal { get; set; }
        public List<MasterDetails> MasterDetails { get; set; }
    }
    public class MasterDetails
    {
        public int ReceiptID { get; set; }
        public string CusName { get; set; }
        public string EmpName { get; set; }
        public string ReceiptNo { get; set; }
        public double DisInvoice { get; set; }
        public double TotalTax { get; set; }
        public string GrandTotal { get; set; }
        public string Currency { get; set; }        
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public SBCHeader SBCHeader { get; set; }
        public Footer Footer { get; set; }        
        public List<DetailItem> DetailItems { get; set; }
    }
    public class SBCHeader
    {
        public string Logo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Branch { get; set; }
        public string CusName { get; set; }
    }
}
