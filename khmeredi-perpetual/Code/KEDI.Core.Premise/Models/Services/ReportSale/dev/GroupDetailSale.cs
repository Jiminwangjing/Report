using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupDetailSale
    {
        public int ReceiptID { get; set; }
        public string EmpName { get; set; }
        public string ReceiptNo { get; set; }
        public double DisInvoice { get; set; }
        public double TotalTax { get; set; }
        public double GrandTotal { get; set; }
        public string Currency { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }
        public List<DetailItem> DetailItems { get; set; }
    }
    public class DetailItem
    {
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string QtyD { get; set; }
        public string UoM { get; set; }
        public double SalePrice { get; set; }
        public string SalePriceD { get; set; }
        public double DisItem { get; set; }
        public string DisItemD { get; set; }
        public double Total { get; set; }
        public string TotalD { get; set; }
        public string EnglisName { get; set; }
    }
}
