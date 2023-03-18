using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class PrintInvoice
    {
        public string OrderNo { set; get; }
        public string QueueNo { set; get; }
        public string ReceiptNo { set; get; }
        public string TableName { set; get; }
        public string CompanyName { set; get; }
        public string BranchName { set; get; }     
        public string DateIn { set; get; }
        public string DateOut { set; get; }
        public string Tel { set; get; }
        public string Tel2 { set; get; }
        public string Description { set; get; }
        public string Description2 { set; get; }
        public string DiscountRate { set; get; }
        public string DiscountValue { set; get; }
        public string Subtotal { set; get; }
        public string VatRate { set; get; }
        public string VatValue { set; get; }     
        public string GrandTotal { set; get; }
        public string GrandTotalSys { set; get; }
        public string Address { set; get; }
        public string AppliedAmount { set; get; }       
        public string ReceivedAmount { set; get; }
        public string ChangedAmount { set; get; }
        public string ChangedAmountSys { set; get; }
        public string CustomerInfo { set; get; }
        public string PaymentMeans { set; get; }
        public string Remark { set; get; }
        public string TotalItemQty { set; get; }
        public string Freights { set; get; }
        public string PrintType { set; get; }
        public string Logo { set; get; }
        public string UserOrder { set; get; }
        public int ReceiptCount { set; get; }
        public List<PrintLineItem> LineItems { set; get; } = new List<PrintLineItem>();
    }

    public class PrintLineItem
    {
        public string LineID { set; get; }
        public string ParentLineID { set; get; }
        public int ItemID { set; get; }
        public string ItemName { set; get; }
        public string ItemName2 { set; get; }
        public string PrintQty { set; get; }
        public string Comment { set; get; }
        public string Qty { set; get; }
        public string UoM { set; get; }
        public string UnitPrice { set; get; }
        public string DiscountRate { set; get; }
        public string DiscountValue { set; get; }
        public string TaxRate { set; get; }
        public string TaxValue { set; get; }
        public string Total { set; get; }
        public string PrinterName { set; get; }
        public string ItemType { set; get; }
    }
}
